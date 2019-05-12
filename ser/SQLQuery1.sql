
-- Create the table in the specified schema
CREATE TABLE UserNotType
(
    Id INT IDENTITY NOT NULL, -- primary key column
    NameUser NVARCHAR(MAX) NOT NULL,
    Password NVARCHAR(MAX) NOT NULL,    
	index_in_list int null,
	PRIMARY KEY(Id)    
)
GO
CREATE TABLE _Room
(
	TableId  INT IDENTITY NOT NULL,
	NameRoom NVARCHAR(MAX) NOT NULL,
	PRIMARY KEY(TableId)
)
GO
CREATE TABLE _User_In_Room
(
	TableId [INT] IDENTITY NOT NULL,
	Room_U [INT] NOT NULL,
	User_U [INT] NOT NULL,
	PRIMARY KEY(TableId),
	FOREIGN KEY (Room_U) REFERENCES _Room (TableId),
	FOREIGN KEY (User_U) REFERENCES UserNotType (Id)    
)
GO	
CREATE TABLE message_on_room
(
	TableId [INT] IDENTITY NOT NULL,
	text_mess NVARCHAR(MAX),
	time_mess DATETIME,
	Room_U [INT] NOT NULL,
	PRIMARY KEY(TableId),
	FOREIGN KEY (Room_U) REFERENCES _User_In_Room (TableId)
)
GO 
INSERT INTO dbo._Room VALUES ('Cardinal')

