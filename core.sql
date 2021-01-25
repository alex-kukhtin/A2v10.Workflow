
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
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'A2v10.Workflow' and TABLE_NAME=N'Catalog')
begin
	create table [A2v10.Workflow].[Catalog]
	(
		[Id] nvarchar(255) not null,
		[Format] nvarchar(32) not null,
		[Body] nvarchar(max) null,
		[Thumb] varbinary(max) null,
		ThumbFormat nvarchar(32) null,
		DateCreated datetime not null constraint DF_Catalog_DateCreated default(getutcdate()),
		constraint PK_Catalog primary key nonclustered (Id)
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
if not exists (select 1 from sys.indexes where [object_id]=object_id(N'[A2v10.Workflow].Instances') and [name]=N'IDX_Id_WorkflowId') begin
	create unique index IDX_Id_WorkflowId on [A2v10.Workflow].Instances (Id, [WorkflowId]);
end;
go
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'A2v10.Workflow' and TABLE_NAME=N'InstanceVariablesInt')
begin
	create table [A2v10.Workflow].[InstanceVariablesInt]
	(
		InstanceId uniqueidentifier not null,
		[Name] nvarchar(255) not null,
		constraint PK_InstanceVariablesInt primary key clustered (InstanceId, [Name]),
		[WorkflowId] nvarchar(255) not null,
		constraint FK_InstanceVariablesInt_PK foreign key (InstanceId, [WorkflowId]) references [A2v10.Workflow].Instances (Id, [WorkflowId]),
		[Value] bigint null
	);
	create index IDX_InstanceVariablesInt on [A2v10.Workflow].[InstanceVariablesInt] ([WorkflowId], [Name], [Value]);
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
create or alter procedure [A2v10.Workflow].[Catalog.Load]
@UserId bigint = null,
@Id nvarchar(255)
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;
	select Id, Body, [Format] from [A2v10.Workflow].[Catalog] where Id=@Id;
end
go
------------------------------------------------
create or alter procedure [A2v10.Workflow].[Catalog.Save]
@UserId bigint = null,
@Id nvarchar(255),
@Body nvarchar(max),
@Format nvarchar(32)
as
begin
	set nocount on;
	set transaction isolation level read committed;
	update [A2v10.Workflow].[Catalog] set Body = @Body where Id=@Id;
	if @@rowcount = 0
		insert into [A2v10.Workflow].[Catalog] (Id, [Format], Body) 
		values (@Id, @Format, @Body)
end
go
------------------------------------------------
create or alter procedure [A2v10.Workflow].[Catalog.Publish]
@UserId bigint = null,
@Id nvarchar(255)
as
begin
	set nocount on;
	set transaction isolation level read committed;

	declare @retval table(Id nvarchar(255), [Version] int);

	insert into [A2v10.Workflow].Workflows (Id, [Format], [Text], [Version])
	output inserted.Id, inserted.[Version] into @retval(Id, [Version])
	select Id, [Format], [Body], [Version] = 
	(select isnull(max([Version]) + 1, 1) from [A2v10.Workflow].Workflows where Id=@Id)
	from [A2v10.Workflow].[Catalog] where Id=@Id;

	select Id, [Version] from @retval;
end
go
------------------------------------------------
create or alter procedure [A2v10.Workflow].[Workflows.Index]
@UserId bigint = null
as
begin
	set nocount on;
	set transaction isolation level read uncommitted;

	select [Workflows!TWorkflow!Array] = null, [Id!!Id] = c.Id, 
		c.[DateCreated], c.[Format],
		InstanceCount = (select count(*) from [A2v10.Workflow].Instances i 
			where WorkflowId = c.Id),
		[Version] = (select max([Version]) from [A2v10.Workflow].Workflows w where w.Id = c.Id)
	from [A2v10.Workflow].[Catalog] c 
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


/*
select * from [A2v10.Workflow].Workflows order by DateCreated desc

insert into [A2v10.Workflow].[Catalog] (Id, Body, [Format])
select Id, [Text], [Format] from [A2v10.Workflow].Workflows where Id=N'Parallel_2' and [Version] = 1

--insert into [A2v10.Workflow].Workflows (Id, [Version], [Format]) values (N'First BPMN', 1, N'xaml')
*/