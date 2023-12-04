package enums

type UpdateStatus int

const (
	New UpdateStatus = iota
	Sending
	Sent
	Fault
	Accepted
	Rejected
	Timeout
)
