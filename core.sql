------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.SCHEMATA where SCHEMA_NAME=N'a2wf')
begin
	exec sp_executesql N'create schema a2wf';
end
go
------------------------------------------------
grant execute on schema ::a2wf to public;
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'a2wf' and TABLE_NAME=N'Workflows')
begin
	create table a2wf.Workflows
	(
		[Id] nvarchar(255) not null,
		[Version] int not null,
		[Format] nvarchar(32) not null,
		[Text] nvarchar(max) null,
		DateCreated datetime not null constraint DF_Workflows_DateCreated default(getutcdate()),
		constraint PK_Workflows primary key clustered (Id, [Version]) with (fillfactor = 70)
	);
end
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'a2wf' and TABLE_NAME=N'Catalog')
begin
	create table a2wf.[Catalog]
	(
		[Id] nvarchar(255) not null,
		[Format] nvarchar(32) not null,
		[Body] nvarchar(max) null,
		[Thumb] varbinary(max) null,
		ThumbFormat nvarchar(32) null,
		DateCreated datetime not null constraint DF_Catalog_DateCreated default(getutcdate()),
		constraint PK_Catalog primary key clustered (Id)
	);
end
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'a2wf' and TABLE_NAME=N'Instances')
begin
	create table a2wf.Instances
	(
		Id uniqueidentifier not null 
			constraint PK_Instances primary key clustered with (fillfactor = 70),
		Parent uniqueidentifier null
			constraint FK_Instances_Parent_Workflows foreign key references a2wf.Instances(Id),
		[WorkflowId] nvarchar(255) not null,
		[Version] int not null,
		[State] nvarchar(max) null,
		[ExecutionStatus] nvarchar(255) null,
		DateCreated datetime not null constraint DF_Instances_DateCreated default(getutcdate()),
		DateModified datetime not null constraint DF_Workflows_Modified default(getutcdate()),
		Lock uniqueidentifier null,
		LockDate datetime null,
		constraint FK_Instances_WorkflowId_Workflows foreign key (WorkflowId, [Version]) 
			references a2wf.Workflows(Id, [Version])
	);
	create unique index IDX_Instances_WorkflowId_Id on a2wf.Instances (WorkflowId, Id) with (fillfactor = 70);
end
go
------------------------------------------------
-- TODO: remove
if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA=N'a2wf' and TABLE_NAME=N'Instances' and COLUMN_NAME=N'Lock')
begin
	alter table a2wf.Instances add Lock uniqueidentifier null;
	alter table a2wf.Instances add LockDate datetime null;
end
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'a2wf' and TABLE_NAME=N'InstanceVariablesInt')
begin
	create table a2wf.InstanceVariablesInt
	(
		InstanceId uniqueidentifier not null,
		[Name] nvarchar(255) not null,
		constraint PK_InstanceVariablesInt primary key clustered (InstanceId, [Name]) with (fillfactor = 70),
		WorkflowId nvarchar(255) not null,
		constraint FK_InstanceVariablesInt_PK foreign key ([WorkflowId], InstanceId) references a2wf.Instances (WorkflowId, Id),
		[Value] bigint null
	);
	create index IDX_InstanceVariablesInt_WNV on a2wf.InstanceVariablesInt (WorkflowId, [Name], [Value]) with (fillfactor = 70);
end
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'a2wf' and TABLE_NAME=N'InstanceVariablesGuid')
begin
	create table a2wf.InstanceVariablesGuid
	(
		InstanceId uniqueidentifier not null,
		[Name] nvarchar(255) not null,
		constraint PK_InstanceVariablesGuid primary key clustered (InstanceId, [Name]) with (fillfactor = 70),
		WorkflowId nvarchar(255) not null,
		constraint FK_InstanceVariablesGuid_PK foreign key ([WorkflowId], InstanceId) references a2wf.Instances (WorkflowId, Id),
		[Value] uniqueidentifier null
	);
	create index IDX_InstanceVariablesGuid_WNV on a2wf.InstanceVariablesGuid (WorkflowId, [Name], [Value]) with (fillfactor = 70);
end
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'a2wf' and TABLE_NAME=N'InstanceVariablesString')
begin
	create table a2wf.InstanceVariablesString
	(
		InstanceId uniqueidentifier not null,
		[Name] nvarchar(255) not null,
		constraint PK_InstanceVariablesString primary key clustered (InstanceId, [Name]) with (fillfactor = 70),
		WorkflowId nvarchar(255) not null,
		constraint FK_InstanceVariablesString_PK foreign key ([WorkflowId], InstanceId) references a2wf.Instances (WorkflowId, Id),
		[Value] nvarchar(255) null
	);
	create index IDX_InstanceVariablesString_WNV on a2wf.InstanceVariablesString (WorkflowId, [Name], [Value]) with (fillfactor = 70);
end
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'a2wf' and TABLE_NAME=N'InstanceBookmarks')
begin
	create table a2wf.InstanceBookmarks
	(
		InstanceId uniqueidentifier not null,
		[Bookmark] nvarchar(255) not null,
		constraint PK_InstanceBookmarks primary key clustered (InstanceId, [Bookmark]) with (fillfactor = 70),
		[WorkflowId] nvarchar(255) not null,
		constraint FK_InstanceBookmarks_PK foreign key (WorkflowId, InstanceId) references a2wf.Instances (WorkflowId, Id),
	);
	create index IDX_InstanceBookmarks_WB on a2wf.InstanceBookmarks (WorkflowId, Bookmark) with (fillfactor = 70);
end
go
------------------------------------------------
if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'a2wf' and TABLE_NAME=N'InstanceTrack')
begin
	create table a2wf.InstanceTrack
	(
		Id bigint identity(100, 1) not null
			constraint PK_InstanceTrack primary key clustered,
		InstanceId uniqueidentifier not null,
		RecordNumber int not null,
		EventTime datetime not null
			constraint DF_InstanceTrack_EventTime default(getutcdate()),
		[Type] nvarchar(32),
		[Action] nvarchar(32),
		[Message] nvarchar(max)
	);
	create index IDX_InstanceTrack_InstanceId on a2wf.InstanceTrack (InstanceId) with (fillfactor = 70);
end
go
------------------------------------------------
create or alter procedure a2wf.[Workflow.Publish]
@UserId bigint = null,
@Id nvarchar(255),
@Format nvarchar(32),
@Text nvarchar(max)
as
begin
	set nocount on;
	set transaction isolation level read committed;
	
	insert into a2wf.Workflows (Id, [Format], [Text], [Version])
	output inserted.Id, inserted.[Version]
	select v.Id, v.[Format], v.[Text], coalesce(cs.[Version], 0) + 1
	from (values (@Id, @Format, @Text)) v (Id, [Format], [Text])
	left join (
		select c.Id, c.[Version], c.[Format], c.[Text], c.DateCreated,
			RN=row_number() over (partition by c.Id order by c.[Version] desc)
		from a2wf.Workflows c
	) cs on cs.Id=v.Id and cs.RN=1
	where not (v.[Format]=cs.[Format] and v.[Text]=cs.[Text]);

end
go
------------------------------------------------
create or alter procedure a2wf.[Workflow.Load]
@UserId bigint = null,
@Id nvarchar(255),
@Version int = 0
as
begin
	set nocount on;
	set transaction isolation level read committed;
	
	select top (1) [Id], [Format], [Version], [Text]
	from a2wf.Workflows
	where Id=@Id and (@Version=0 or [Version]=@Version)
	order by [Version] desc;
end
go
------------------------------------------------
create or alter procedure a2wf.[Instance.Load]
@UserId bigint = null,
@Id uniqueidentifier
as
begin
	set nocount on;
	set transaction isolation level read committed;
	declare @inst table(id uniqueidentifier);

	update a2wf.Instances set Lock=newid(), LockDate = getutcdate()
	output inserted.Id into @inst(Id)
	where Id=@Id and Lock is null;

	select [Instance!TInstance!Object] = null, [Id!!Id] = i.Id, [WorkflowId], [Version], [State], 
		ExecutionStatus, Lock
	from @inst t inner join a2wf.Instances i on t.Id = i.Id
	where t.Id=@Id;
end
go
------------------------------------------------
create or alter procedure a2wf.[Instance.Create]
@UserId bigint = null,
@Id uniqueidentifier,
@Parent uniqueidentifier,
@Version int = 0,
@WorkflowId nvarchar(255),
@ExecutionStatus nvarchar(255)
as
begin
	set nocount on;
	set transaction isolation level read committed;
	set xact_abort on;
	insert into a2wf.Instances(Id, Parent, WorkflowId, [Version], ExecutionStatus)
	values (@Id, @Parent, @WorkflowId, @Version, @ExecutionStatus);
end
go
------------------------------------------------
drop procedure if exists a2wf.[Instance.Update];
go
------------------------------------------------
if exists(select * from INFORMATION_SCHEMA.DOMAINS where DOMAIN_SCHEMA = N'a2wf' and DOMAIN_NAME = N'Instance.TableType')
	drop type a2wf.[Instance.TableType]
go
------------------------------------------------
create type a2wf.[Instance.TableType] as table
(
	[GUID] uniqueidentifier,
	Id uniqueidentifier,
	WorkflowId nvarchar(255),
	[ExecutionStatus] nvarchar(255) null,
	Lock uniqueidentifier,
	[State] nvarchar(max)
)
go
------------------------------------------------
if exists(select * from INFORMATION_SCHEMA.DOMAINS where DOMAIN_SCHEMA = N'a2wf' and DOMAIN_NAME = N'Variables.TableType')
	drop type a2wf.[Variables.TableType]
go
------------------------------------------------
create type a2wf.[Variables.TableType] as table
(
	[GUID] uniqueidentifier,
	ParentGUID uniqueidentifier
)
go
------------------------------------------------
if exists(select * from INFORMATION_SCHEMA.DOMAINS where DOMAIN_SCHEMA = N'a2wf' and DOMAIN_NAME = N'VariableInt.TableType')
	drop type a2wf.[VariableInt.TableType]
go
------------------------------------------------
create type a2wf.[VariableInt.TableType] as table
(
	ParentGUID uniqueidentifier,
	[Name] nvarchar(255),
	[Value] bigint
)
go
------------------------------------------------
if exists(select * from INFORMATION_SCHEMA.DOMAINS where DOMAIN_SCHEMA = N'a2wf' and DOMAIN_NAME = N'VariableGuid.TableType')
	drop type a2wf.[VariableGuid.TableType]
go
------------------------------------------------
create type a2wf.[VariableGuid.TableType] as table
(
	ParentGUID uniqueidentifier,
	[Name] nvarchar(255),
	[Value] uniqueidentifier
)
go
------------------------------------------------
if exists(select * from INFORMATION_SCHEMA.DOMAINS where DOMAIN_SCHEMA = N'a2wf' and DOMAIN_NAME = N'VariableString.TableType')
	drop type a2wf.[VariableString.TableType]
go
------------------------------------------------
create type a2wf.[VariableString.TableType] as table
(
	ParentGUID uniqueidentifier,
	[Name] nvarchar(255),
	[Value] nvarchar(255)
)
go
------------------------------------------------
if exists(select * from INFORMATION_SCHEMA.DOMAINS where DOMAIN_SCHEMA = N'a2wf' and DOMAIN_NAME = N'Bookmarks.TableType')
	drop type a2wf.[Bookmarks.TableType]
go
------------------------------------------------
create type a2wf.[Bookmarks.TableType] as table
(
	[GUID] uniqueidentifier,
	ParentGUID uniqueidentifier
)
go
------------------------------------------------
if exists(select * from INFORMATION_SCHEMA.DOMAINS where DOMAIN_SCHEMA = N'a2wf' and DOMAIN_NAME = N'InstanceBookmarks.TableType')
	drop type a2wf.[InstanceBookmarks.TableType]
go
------------------------------------------------
create type a2wf.[InstanceBookmarks.TableType] as table
(
	ParentGUID uniqueidentifier,
	[Bookmark] nvarchar(255)
)
go
------------------------------------------------
if exists(select * from INFORMATION_SCHEMA.DOMAINS where DOMAIN_SCHEMA = N'a2wf' and DOMAIN_NAME = N'InstanceTrack.TableType')
	drop type a2wf.[InstanceTrack.TableType]
go
------------------------------------------------
create type a2wf.[InstanceTrack.TableType] as table
(
	ParentGUID uniqueidentifier,
	RecordNumber int,
	EventTime datetime,
	[Type] int,
	[Action] int,
	[Message] nvarchar(max)
)
go
------------------------------------------------
create or alter procedure a2wf.[Instance.Metadata]
as
begin
	declare @Instance a2wf.[Instance.TableType];
	declare @Variables a2wf.[Variables.TableType];
	declare @VariableInt a2wf.[VariableInt.TableType];
	declare @VariableGuid a2wf.[VariableGuid.TableType];
	declare @VariableString a2wf.[VariableString.TableType];
	declare @Book a2wf.[Bookmarks.TableType];
	declare @Bookmarks a2wf.[InstanceBookmarks.TableType];
	declare @TrackRecords a2wf.[InstanceTrack.TableType];

	select [Instance!Instance!Metadata] = null, * from @Instance;
	select [Variables!Instance.Variables!Metadata] = null, * from @Variables;
	select [IntVariables!Instance.Variables.BigInt!Metadata] = null, * from @VariableInt;
	select [GuidVariables!Instance.Variables.Guid!Metadata] = null, * from @VariableGuid;
	select [StringVariables!Instance.Variables.String!Metadata] = null, * from @VariableString;
	select [Book!Instance.Bookmarks!Metadata] = null, * from @Book;
	select [Bookmarks!Instance.Bookmarks.Items!Metadata] = null, * from @Bookmarks;
	select [TrackRecords!Instance.TrackRecords!Metadata] = null, * from @TrackRecords;
end
go
------------------------------------------------
create or alter procedure a2wf.[Instance.Update]
@UserId bigint = null,
@Instance a2wf.[Instance.TableType] readonly,
@Variables a2wf.[Variables.TableType] readonly,
@IntVariables a2wf.[VariableInt.TableType] readonly,
@GuidVariables a2wf.[VariableGuid.TableType] readonly,
@StringVariables a2wf.[VariableString.TableType] readonly,
@Book a2wf.[Bookmarks.TableType] readonly,
@Bookmarks a2wf.[InstanceBookmarks.TableType] readonly,
@TrackRecords a2wf.[InstanceTrack.TableType] readonly
as
begin
	set nocount on;
	set transaction isolation level read committed;
	set xact_abort on;

	/*
	declare @xml nvarchar(max);
	set @xml = (select * from @TrackRecords for xml auto);
	throw 60000, @xml, 0;
	*/

	begin tran;
	
	declare @rtable table (id uniqueidentifier);
	with ti as (
		select t.Id, t.[State], t.DateModified, t.ExecutionStatus, t.Lock, t.LockDate
		from a2wf.Instances t
		inner join @Instance p on p.Id = t.Id and t.Lock = p.Lock
	)
	merge ti as t
	using @Instance as s
	on s.Id = t.Id
	when matched then update set 
		t.[State] = s.[State],
		t.DateModified = getutcdate(),
		t.ExecutionStatus = s.ExecutionStatus,
		t.Lock = null,
		t.LockDate = null
	output inserted.Id into @rtable;
	
	if not exists(select id from @rtable)
	begin
		declare @wfid nvarchar(255);
		select @wfid = cast(Id as nvarchar(255)) from @Instance;
		raiserror(N'Failed to update workflow (id = %s', 16, -1, @wfid) with nowait;
	end;

	with t as (
		select tt.*
		from a2wf.InstanceVariablesInt tt
		inner join @Instance si on si.Id = tt.InstanceId
	)
	merge t
	using (
		select si.WorkflowId, InstanceId=si.Id, siv.*
		from @Instance si
		inner join @Variables sv on sv.ParentGUID=si.[GUID]
		inner join @IntVariables siv on siv.ParentGUID=sv.[GUID]
	) as s
	on t.[Name] = s.[Name] and t.InstanceId = s.InstanceId and t.WorkflowId = s.WorkflowId
	when matched then update set 
		t.[Value] = s.[Value]
	when not matched by target then insert
		(InstanceId, [Name], WorkflowId, [Value]) values
		(s.InstanceId, s.[Name], s.WorkflowId, s.[Value])
	when not matched by source then delete;

	with t as (
		select tt.*
		from a2wf.InstanceVariablesGuid tt
		inner join @Instance si on si.Id=tt.InstanceId
	)
	merge t
	using (
		select si.WorkflowId, InstanceId=si.Id, siv.*
		from @Instance si
		inner join @Variables sv on sv.ParentGUID=si.[GUID]
		inner join @GuidVariables siv on siv.ParentGUID=sv.[GUID]
	) as s
	on t.[Name] = s.[Name] and t.InstanceId = s.InstanceId and t.WorkflowId = s.WorkflowId
	when matched then update set 
		t.[Value] = s.[Value]
	when not matched by target then insert
		(InstanceId, [Name], WorkflowId, [Value]) values
		(s.InstanceId, s.[Name], s.WorkflowId, s.[Value])
	when not matched by source then delete;

	with t as (
		select tt.*
		from a2wf.InstanceVariablesString tt
		inner join @Instance si on si.Id=tt.InstanceId
	)
	merge t
	using (
		select si.WorkflowId, InstanceId=si.Id, siv.*
		from @Instance si
		inner join @Variables sv on sv.ParentGUID=si.[GUID]
		inner join @StringVariables siv on siv.ParentGUID=sv.[GUID]
	) as s
	on t.[Name] = s.[Name] and t.InstanceId = s.InstanceId and t.WorkflowId = s.WorkflowId
	when matched then update set 
		t.[Value] = s.[Value]
	when not matched by target then insert
		(InstanceId, [Name], WorkflowId, [Value]) values
		(s.InstanceId, s.[Name], s.WorkflowId, s.[Value])
	when not matched by source then delete;

	with t as (
		select tt.*
		from a2wf.InstanceBookmarks tt
		inner join @Instance si on si.Id=tt.InstanceId
	)
	merge t
	using (
		select si.WorkflowId, InstanceId=si.Id, sib.*
		from @Instance si
		inner join @Book sb on sb.ParentGUID=si.[GUID]
		inner join @Bookmarks sib on sib.ParentGUID=sb.[GUID]
	) as s
	on t.[Bookmark] = s.[Bookmark] and t.InstanceId = s.InstanceId and t.WorkflowId = s.WorkflowId
	when not matched by target then insert
		(InstanceId, [Bookmark], WorkflowId) values
		(s.InstanceId, s.[Bookmark], s.WorkflowId)
	when not matched by source then delete;

	insert into a2wf.InstanceTrack(InstanceId, RecordNumber, EventTime, [Type], [Action], [Message])
	select i.Id, RecordNumber, EventTime, [Type], [Action], [Message] from 
	@TrackRecords r inner join @Instance i on r.ParentGUID = i.[GUID];

	commit tran;
end
go
------------------------------------------------
create or alter procedure a2wf.[Catalog.Load]
@UserId bigint = null,
@Id nvarchar(255)
as
begin
	set nocount on;
	set transaction isolation level read committed;
	select Id, Body, [Format] from a2wf.[Catalog] where Id=@Id;
end
go
------------------------------------------------
create or alter procedure a2wf.[Catalog.Save]
@UserId bigint = null,
@Id nvarchar(255),
@Body nvarchar(max),
@Format nvarchar(32)
as
begin
	set nocount on;
	set transaction isolation level read committed;
	update a2wf.[Catalog] set Body = @Body where Id=@Id;
	if @@rowcount = 0
		insert into a2wf.[Catalog] (Id, [Format], Body) 
		values (@Id, @Format, @Body)
end
go
------------------------------------------------
create or alter procedure a2wf.[Catalog.Publish]
@UserId bigint = null,
@Id nvarchar(255)
as
begin
	set nocount on;
	set transaction isolation level read committed;

	declare @retval table(Id nvarchar(255), [Version] int);

	insert into a2wf.Workflows (Id, [Format], [Text], [Version])
	output inserted.Id, inserted.[Version] into @retval(Id, [Version])
	select Id, [Format], [Body], [Version] = 
	(select isnull(max([Version]) + 1, 1) from a2wf.Workflows where Id=@Id)
	from a2wf.[Catalog] where Id=@Id;

	select Id, [Version] from @retval;
end
go
------------------------------------------------
create or alter procedure a2wf.[Workflows.Index]
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
------------------------------------------------
create or alter procedure a2wf.[Instances.Index]
@UserId bigint = null
as
begin
	set nocount on;
	set transaction isolation level read committed;

	select [Instances!TWorkflow!Array] = null, 
		[Id!!Id] = i.Id, i.WorkflowId, i.[Version], i.[DateCreated], i.DateModified,
		i.[State], i.ExecutionStatus
	from a2wf.Instances i 
	order by DateModified desc
end
go