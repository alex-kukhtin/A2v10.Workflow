------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.SCHEMATA where SCHEMA_NAME=N'a2wfui')
begin
	exec sp_executesql N'create schema a2wfui authorization dbo';
end
go
------------------------------------------------
grant execute on schema ::a2wfui to public;
go
------------------------------------------------
create or alter procedure a2wfui.[Instances.Index]
@UserId bigint = null
as
begin
	set nocount on;
	set transaction isolation level read committed;

	select [Instances!TWorkflow!Array] = null, 
		[Id!!Id] = i.Id, i.WorkflowId, i.[Version], i.[DateCreated], i.DateModified,
		i.[State], i.ExecutionStatus, i.Lock, i.LockDate
	from a2wf.Instances i 
	order by DateModified desc
end
go
------------------------------------------------
create or alter procedure a2wfui.[Instance.Load]
@UserId bigint = null,
@Id uniqueidentifier
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	select [Instance!TInstance!Object] = null, [Id!!Id] = i.Id, [WorkflowId], [Version], [State], 
		ExecutionStatus, Lock, LockDate, [Track!TTrack!Array] = null
	from a2wf.Instances i
	where i.Id=@Id;

	select [!TTrack!Array] = null, [!TInstance.Track!ParentId] = InstanceId, 
		t.Activity, t.[Action],
		t.RecordNumber, [EventTime!!Utc] = t.EventTime, t.[Message]
	from a2wf.InstanceTrack t
	where InstanceId=@Id
	order by t.EventTime;
end
go
------------------------------------------------
create or alter procedure a2wfui.[Workflows.Index]
@UserId bigint = null
as
begin
	set nocount on;
	set transaction isolation level read committed;

	select [Workflows!TWorkflow!Array] = null, [Id!!Id] = c.Id, 
		c.[DateCreated], c.[Format],
		InstanceCount = (select count(*) from a2wf.Instances i 
			where WorkflowId = c.Id),
		[Version] = (select max([Version]) from a2wf.Workflows w where w.Id = c.Id)
	from a2wf.[Catalog] c 
	order by DateCreated desc
end
go
drop procedure if exists a2wfui.[WorkflowList.Update];
go



------------------------------------------------
if exists(select * from INFORMATION_SCHEMA.DOMAINS where DOMAIN_SCHEMA = N'a2wfui' and DOMAIN_NAME = N'WorkflowList.TableType')
	drop type a2wfui.[WorkflowList.TableType]
go
------------------------------------------------
create type a2wfui.[WorkflowList.TableType] as table
(
	RowNumber int,
	[DateCreated] datetime,
	[Format] nvarchar(255),
	Id nvarchar(255)
)
go
create or alter procedure a2wfui.[WorkflowList.Metadata]
as
begin
	declare @List a2wfui.[WorkflowList.TableType];
	select [List!Workflows!Metadata] = null, * from @List;
end
go
------------------------------------------------
create or alter procedure a2wfui.[WorkflowList.Update]
@UserId bigint = null,
@List a2wfui.[WorkflowList.TableType] readonly
as
begin
	set nocount on;
	set transaction isolation level read committed;

	select [Workflows!TWorkflow!Array] = null,
		[Id!!Id] = c.Id,
		c.[DateCreated], c.[Format],
		InstanceCount = (select count(*) from a2wf.Instances i 
			where WorkflowId = c.Id),
		[Version] = (select max([Version]) from a2wf.Workflows w where w.Id = c.Id)
	from @List c
	order by c.RowNumber
end
go
------------------------------------------------
create or alter procedure a2wfui.[Instance.Unlock]
@UserId bigint = null,
@Id uniqueidentifier
as
begin
	set nocount on;
	set transaction isolation level read committed;

	update a2wf.Instances set Lock = null, LockDate= null where Id=@Id;
end
go
