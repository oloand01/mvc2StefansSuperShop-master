USE [aspnet-StefanShopWeb-E3CED2D5-A95E-4B2D-A5F0-69A4F8C0D341]
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[Wishinglist]'))
BEGIN 
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Wishinglist](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ProductID] [int] NOT NULL,
 CONSTRAINT [PK_Wishinglist] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[Wishinglist]  WITH CHECK ADD  CONSTRAINT [FK_Wishinglist_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])

ALTER TABLE [dbo].[Wishinglist] CHECK CONSTRAINT [FK_Wishinglist_AspNetUsers]


ALTER TABLE [dbo].[Wishinglist]  WITH CHECK ADD  CONSTRAINT [FK_Wishinglist_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])


ALTER TABLE [dbo].[Wishinglist] CHECK CONSTRAINT [FK_Wishinglist_Products]

END
GO