import logging
import azure.functions as func
from service import AggregatedLogsProcessor

app = func.FunctionApp()

@app.function_name(name="ProcessAggregationLogsTimer")
@app.schedule(schedule="* * * * *", arg_name="myTimer", run_on_startup=True, #0 5 * * * 
              use_monitor=False) 
def ProcessAggregationLogsTimer(myTimer: func.TimerRequest) -> None:
    if myTimer.past_due:
        logging.info('The timer is past due!')

    aggregated_logs_processor = AggregatedLogsProcessor()
    aggregated_logs_processor.perform_daily_processing()

    logging.info('Daily aggregation processing has been performed.')
