import subprocess
import platform
import os

system_info = platform.system()
current_directory = os.getcwd()

if system_info == "Windows":
    subprocess.run(["powershell", f"{current_directory}\\ps_startup.ps1"], shell=True, check=True)
elif system_info == "Linux" or system_info == "Darwin":
    subprocess.run([f"{current_directory}/bs_startup.sh"], check=True)