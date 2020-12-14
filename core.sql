
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.SCHEMATA where SCHEMA_NAME=N'A2v10.Workflow')
begin
	exec sp_executesql N'create schema [A2v10.Workflow]';
end
go
------------------------------------------------
grant execute on schema ::[A2v10.Workflow] to public;
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'A2v10.Workflow' and TABLE_NAME=N'Workflows')
begin
	create table [A2v10.Workflow].[Workflows]
	(
		[Id] nvarchar(255) not null,
		[Version] int not null,
		[Format] nvarchar(32) not null,
		[Text] nvarchar(max) null,
		DateCreated datetime not null constraint DF_Workflows_DateCreated default(getutcdate()),
		constraint PK_Workflows primary key nonclustered (Id, [Version])
	);
end
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'A2v10.Workflow' and TABLE_NAME=N'Instances')
begin
	create table [A2v10.Workflow].[Instances]
	(
		Id uniqueidentifier not null 
			constraint PK_Instances primary key nonclustered,
		Parent uniqueidentifier null
			constraint FK_Instances_Parent_Workflows foreign key references [A2v10.Workflow].Instances(Id),
		[WorkflowId] nvarchar(255) not null,
		[Version] int not null,
		[State] nvarchar(max) null,
		[ExecutionStatus] nvarchar(255) null,
		DateCreated datetime not null constraint DF_Instances_DateCreated default(getutcdate()),
		DateModified datetime not null constraint DF_Workflows_Modified default(getutcdate()),
		constraint FK_Instances_WorkflowId_Workflows foreign key (WorkflowId, [Version]) 
			references [A2v10.Workflow].Workflows(Id, [Version])
	);
end
go
------------------------------------------------
create or alter procedure [A2v10.Workflow].[Workflow.Publish]
@UserId bigint = null,
@Id nvarchar(255),
@Format nvarchar(32),
@Text nvarchar(max)
as
begin
	set nocount on;
	set transaction isolation level read committed;

	declare @rtable table(Id nvarchar(255), [Version] int);
	
	insert into [A2v10.Workflow].Workflows (Id, [Format], [Text], [Version])
	output inserted.Id, inserted.[Version] into @rtable
	values (@Id, @Format, @Text,
		(select isnull(max([Version]), 0) + 1 from [A2v10.Workflow].Workflows where Id=@Id)
	);

	select [Id], [Version] from @rtable;
end
go
------------------------------------------------
create or alter procedure [A2v10.Workflow].[Workflow.Load]
@UserId bigint = null,
@Id nvarchar(255),
@Version int = 0
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;
	if @Version = 0
		select @Version = max([Version]) from [A2v10.Workflow].Workflows where Id=@Id;
	select [Id], [Format], [Version], [Text] from [A2v10.Workflow].Workflows 
	where Id=@Id and [Version]=@Version;
end
go
------------------------------------------------
create or alter procedure [A2v10.Workflow].[Instance.Load]
@UserId bigint = null,
@Id uniqueidentifier
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;
	select [Instance!TInstance!Object] = null, [Id!!Id] = Id, [WorkflowId], [Version], [State], 
		ExecutionStatus, 
		DateCreated, DateModified
	from [A2v10.Workflow].Instances 
	where Id=@Id;
end
go
------------------------------------------------
create or alter procedure [A2v10.Workflow].[Instance.Save]
@UserId bigint = null,
@Id uniqueidentifier,
@Parent uniqueidentifier,
@Version int,
@WorkflowId nvarchar(255),
@State nvarchar(max)
as
begin
	set nocount on;
	set transaction isolation level read committed;
	update [A2v10.Workflow].Instances set
		[State] = @State, DateModified=getutcdate()
	where Id=@Id;
	if @@rowcount = 0
		insert into [A2v10.Workflow].Instances 
			(Id, Parent, WorkflowId, [Version], [State])
		values
			(@Id, @Parent, @WorkflowId, @Version, @State);
end
go
------------------------------------------------
create or alter procedure [A2v10.Workflow].[Workflows.Index]
@UserId bigint = null
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	with T(Id, Version)
	as (
		select Id, max([Version]) from [A2v10.Workflow].Workflows
		group by Id
	)
	select [Workflows!TWorkflow!Array] = null, [Id!!Id] = w.Id, w.[Version], 
		w.[DateCreated], w.[Format],
		InstanceCount = (select count(*) from [A2v10.Workflow].Instances i 
			where WorkflowId = w.Id and i.[Version] = w.[Version])
	from T inner join [A2v10.Workflow].Workflows w on T.Id = w.Id and T.[Version] = w.[Version]
	order by DateCreated desc
end
go
------------------------------------------------
create or alter procedure [A2v10.Workflow].[Instances.Index]
@UserId bigint = null
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	select [Instances!TWorkflow!Array] = null, 
		[Id!!Id] = i.Id, i.WorkflowId, i.[Version], i.[DateCreated], i.DateModified,
		i.[State], i.ExecutionStatus
	from [A2v10.Workflow].Instances i 
	order by DateModified desc
end
go

--insert into [A2v10.Workflow].Workflows (Id, [Version], [Format]) values (N'First BPMN', 1, N'xaml')
