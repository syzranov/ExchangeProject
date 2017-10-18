@echo "kill server process.."
@Taskkill /IM Exchange.Server.Cmd.exe /F

@echo "kill all exchange clients processes.."
@Taskkill /IM Exchange.Client.Cmd.exe /F

@echo "kill monitor process.."
@Taskkill /IM Exchange.Monitor.Cmd.exe /F
slip 3
@echo "done"
slip 1
