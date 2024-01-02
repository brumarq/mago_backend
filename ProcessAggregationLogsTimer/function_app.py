import logging
import azure.functions as func
from service import AggregatedLogsProcessor

app = func.FunctionApp()

@app.function_name(name="process_aggregation_logs_timer")
@app.schedule(schedule="0 5 * * *", arg_name="myTimer", run_on_startup=True,
              use_monitor=False) 
def process_aggregation_logs_timer(myTimer: func.TimerRequest) -> None:
    if myTimer.past_due:
        logging.info('The timer is past due!')

    aggregated_logs_processor = AggregatedLogsProcessor()
    aggregated_logs_processor.perform_daily_processing()

    logging.info('Daily aggregation processing has been performed.')
