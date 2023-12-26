import logging
import azure.functions as func
from queries import calculate_and_save_aggregates

app = func.FunctionApp()

@app.function_name(name="CalculateAveragesTimer")
@app.schedule(schedule="* * * * *", arg_name="myTimer", run_on_startup=True,
              use_monitor=False) 
def CalculateAveragesTimer(myTimer: func.TimerRequest) -> None:
    if myTimer.past_due:
        logging.info('The timer is past due!')

    calculate_and_save_aggregates()

    logging.info('Python timer trigger function executed.')
