CREATE DATABASE EMMAIL;
USE EMMAIL;

CREATE TABLE PATTERN(
	ID int identity(1,1),
	NAME varchar(255) not null,
	CONTENT varchar(255) not null,
	HTML bit not null,
	CONSTRAINT [PK_PATTERN] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [IX_PATTERN] UNIQUE NONCLUSTERED ([NAME] ASC)
);

CREATE TABLE QUEUE(
	ID int identity(1,1),
	PATTERN_ID int not null,
	[TO] varchar(255) not null,
	SUBJECT varchar(255) not null,
	BODY varchar(255) not null,
	CREATIONDATE datetime2(7) null,
	SENDDATE datetime2(7) null,
	SEND bit not null DEFAULT 0,
	CONSTRAINT [PK_QUEUE] PRIMARY KEY CLUSTERED ([ID] ASC),
	FOREIGN KEY (PATTERN_ID) REFERENCES PATTERN(ID)
);

CREATE TABLE SERVER(
	ID int identity(1,1),
	IP varchar(255) not null,
	LOGIN varchar(255) not null,
	PASSWORD varchar(255) null,
	PORT int not null,
	SSL bit not null,
	ENABLE bit not null DEFAULT 1,
	CONSTRAINT [PK_SERVER] PRIMARY KEY CLUSTERED ([ID] ASC)
);
