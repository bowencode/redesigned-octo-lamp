IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231201070648_DatabaseSetup')
BEGIN
    CREATE TABLE [MailingAddresses] (
        [Id] int NOT NULL IDENTITY,
        [Street] nvarchar(max) NULL,
        [City] nvarchar(max) NULL,
        [State] nvarchar(2) NULL,
        [ZipCode] nvarchar(max) NULL,
        CONSTRAINT [PK_MailingAddresses] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231201070648_DatabaseSetup')
BEGIN
    CREATE TABLE [Companies] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [ActiveDate] datetime2 NOT NULL,
        [MailingAddressId] int NULL,
        CONSTRAINT [PK_Companies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Companies_MailingAddresses_MailingAddressId] FOREIGN KEY ([MailingAddressId]) REFERENCES [MailingAddresses] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231201070648_DatabaseSetup')
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [ActiveDate] datetime2 NOT NULL,
        [MailingAddressId] int NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Users_MailingAddresses_MailingAddressId] FOREIGN KEY ([MailingAddressId]) REFERENCES [MailingAddresses] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231201070648_DatabaseSetup')
BEGIN
    CREATE INDEX [IX_Companies_MailingAddressId] ON [Companies] ([MailingAddressId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231201070648_DatabaseSetup')
BEGIN
    CREATE INDEX [IX_Users_MailingAddressId] ON [Users] ([MailingAddressId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231201070648_DatabaseSetup')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231201070648_DatabaseSetup', N'7.0.13');
END;
GO

COMMIT;
GO

