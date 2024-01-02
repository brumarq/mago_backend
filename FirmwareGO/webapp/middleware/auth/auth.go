package auth

import (
	"context"
	jwtmiddleware "github.com/auth0/go-jwt-middleware/v2"
	"github.com/auth0/go-jwt-middleware/v2/jwks"
	"github.com/auth0/go-jwt-middleware/v2/validator"
	"github.com/gin-gonic/gin"
	"log"
	"net/http"
	"net/url"
	"os"
	"strings"
	"time"
)

type CustomClaims struct {
	Scope       string   `json:"scope"`
	Permissions []string `json:"permissions"`
}

func (c CustomClaims) Validate(ctx context.Context) error {
	return nil
}

func (c CustomClaims) HasScope(expectedScope string) bool {
	result := strings.Split(c.Scope, " ")
	for i := range result {
		if result[i] == expectedScope {
			return true
		}
	}

	return false
}

func (c CustomClaims) HasPermission(requiredPermission string) bool {
	for _, p := range c.Permissions {
		if p == requiredPermission {
			return true
		}
	}

	return false
}

// JwtMiddlewareHandler is a generic gin handler function that process http handler functions,
// making http handlers compatible with gin.HandlerFunc
func JwtMiddlewareHandler(middleware func(next http.Handler) http.Handler) gin.HandlerFunc {
	return func(c *gin.Context) {
		handler := middleware(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
			c.Request = r
			c.Next()
		}))
		handler.ServeHTTP(c.Writer, c.Request)

		if c.Writer.Status() == http.StatusUnauthorized ||
			c.Writer.Status() == http.StatusForbidden ||
			c.Writer.Status() == http.StatusInternalServerError {
			// If unauthorized, forbidden or error, abort the Gin context to prevent further processing
			c.Abort()
			return
		}

		c.Next()
	}
}

// EnsureValidJwt is a http handler that validates the origin, algorithm, audience, and custom claims of received
// jwt tokens within requests
func EnsureValidJwt() func(next http.Handler) http.Handler {
	issuerURL, err := url.Parse("https://" + os.Getenv("AUTH0_DOMAIN") + "/")
	if err != nil {
		log.Fatalf("Failed to parse the issuer url: %v", err)
	}

	provider := jwks.NewCachingProvider(issuerURL, 5*time.Minute)

	jwtValidator, err := validator.New(
		provider.KeyFunc,
		validator.RS256,
		issuerURL.String(),
		[]string{os.Getenv("AUTH0_AUDIENCE")},
		validator.WithCustomClaims(
			func() validator.CustomClaims {
				return &CustomClaims{}
			},
		),
		validator.WithAllowedClockSkew(time.Minute),
	)
	if err != nil {
		log.Fatalf("Failed to set up the jwt validator")
	}

	errorHandler := func(w http.ResponseWriter, r *http.Request, err error) {
		log.Printf("Encountered error while validating JWT: %v", err)

		w.Header().Set("Content-Type", "application/json")
		w.WriteHeader(http.StatusUnauthorized)
		w.Write([]byte(`{"message":"Failed to validate JWT."}`))
	}

	middleware := jwtmiddleware.New(
		jwtValidator.ValidateToken,
		jwtmiddleware.WithErrorHandler(errorHandler),
	)

	return middleware.CheckJWT
}

// EnsureAdminPermission is a http handler that validates whether the provided jwt token contains the "admin" permission
// within its payload
func EnsureAdminPermission() func(next http.Handler) http.Handler {
	return func(next http.Handler) http.Handler {
		return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
			claims, ok := r.Context().Value(jwtmiddleware.ContextKey{}).(*validator.ValidatedClaims)
			if !ok || claims == nil {
				http.Error(w, "Error processing JWT claims", http.StatusInternalServerError)
				return
			}

			customClaims := claims.CustomClaims.(*CustomClaims)
			if customClaims == nil || !customClaims.HasPermission("admin") {
				http.Error(w, "Permission denied", http.StatusForbidden)
				return
			}

			next.ServeHTTP(w, r)
		})
	}
}
