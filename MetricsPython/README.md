#### How to start

### Prerequisites 
Make sure that the following aspects are done

    > Install Python (version 3.9-3.11)
    
    > Install pip (should be there upon installation of Python)
    
    > Install virtualenv -> pip install virtualenv

### Start-up
It is recommended to run these commands in a terminal of some sort. Please use Powershell or Bash. The terminal is Visual Studio Code also works.

For just running purposes:

    > python startup.py (will execute all commands below)

For development purposes:

    > python -m venv venv (to create virtual environment)
    
    > .\venv\Scripts\Activate (Windows) || source venv/bin/activate (Linux)
    
    > pip install -r requirements.txt
    
    > $env:FLASK_APP="manage.py" (Powershell) || export FLASK_APP=manage.py (Bash)
    
    > flask run (to start the app)

### Database related commands
Make sure to run the initial migration commands to update the database (from the venv!).
    
    > flask db init (makes "migration" folder)

    > flask db migrate -m "message" (makes a migration with current changes)

    > flask db upgrade (pushes changes from code to database)


### Viewing the app ###
    Open the following url on your browser to view swagger documentation
    http://127.0.0.1:5000/

