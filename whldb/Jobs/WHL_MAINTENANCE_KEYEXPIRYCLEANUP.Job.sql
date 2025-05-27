USE [msdb]
GO

DECLARE @JobID BINARY(16)
SELECT @JobID = CONVERT(uniqueidentifier, job_id) FROM msdb.dbo.sysjobs
WHERE name = N'WHL_MAINTENANCE_KEYEXPIRYCLEANUP';
IF @JobID IS NOT NULL
    EXEC msdb.dbo.sp_delete_job @job_id=@JobID, @delete_unused_schedule=1
GO

DECLARE @JobUsername VARCHAR(32);
SET @JobUsername = CASE
						WHEN @@SERVERNAME LIKE 'SRVR2345%' OR @@SERVERNAME LIKE 'SQL1129%' THEN 'whluser'
						ELSE 'whladmin'
					END

BEGIN TRANSACTION

DECLARE @ReturnCode INT
SELECT @ReturnCode = 0

IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'WHL' AND category_class=1)
BEGIN
	EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'WHL'
	IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'WHL_MAINTENANCE_KEYEXPIRYCLEANUP', 
		@enabled=0, 
		@notify_level_eventlog=2, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'Clean up expired keys.', 
		@category_name=N'WHL', 
		@owner_login_name=@JobUsername, @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Execute clean up of expired keys', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'EXEC dbo.uspMaintenanceKeyExpiryCleanup', 
		@database_name=N'WHL', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'WHL_EVERYMIN', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=4, 
		@freq_subday_interval=1, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=0, 
		@active_start_date=20240727, 
		@active_end_date=99991231, 
		@active_start_time=0, 
		@active_end_time=235959
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

IF @@SERVERNAME LIKE 'SRVR2345%' OR @@SERVERNAME LIKE 'SQL1129%'
BEGIN
	EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @enabled = 1
	IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
END

COMMIT TRANSACTION

GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION

EndSave:
GO