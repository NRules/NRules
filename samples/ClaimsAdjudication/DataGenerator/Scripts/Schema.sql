CREATE TABLE Claim
(
Id INTEGER PRIMARY KEY AUTOINCREMENT,
Status VARCHAR(20),
ClaimType VARCHAR(50),
PatientFirstName VARCHAR(50),
PatientLastName VARCHAR(50),
PatientMiddleName VARCHAR(50),
PatientAddressLine1 VARCHAR(100),
PatientAddressLine2 VARCHAR(100),
PatientAddressCity VARCHAR(50),
PatientAddressState VARCHAR(50),
PatientAddressZip VARCHAR(10),
PatientPhone VARCHAR(15),
PatientDob DATETIME,
PatientSex VARCHAR(10),
PatientRelationship VARCHAR(25),
PatientMaritalStatus VARCHAR(15),
PatientEmploymentStatus VARCHAR(15),
InsuredFirstName VARCHAR(50),
InsuredLastName VARCHAR(50),
InsuredMiddleName VARCHAR(50),
InsuredAddressLine1 VARCHAR(100),
InsuredAddressLine2 VARCHAR(100),
InsuredAddressCity VARCHAR(50),
InsuredAddressState VARCHAR(50),
InsuredAddressZip VARCHAR(10),
InsuredPhone VARCHAR(15),
InsuredDob DATETIME,
InsuredSex VARCHAR(10),
InsuredEmployerName VARCHAR(100),
InsuredPolicyNumber VARCHAR(20),
InsuredPlanName VARCHAR(100)
);

CREATE TABLE ClaimAlert
(
Id INTEGER PRIMARY KEY AUTOINCREMENT,
ClaimId INTEGER,
Severity INTEGER,
RuleName VARCHAR(255),
Message VARCHAR(8000),
FOREIGN KEY (ClaimId) REFERENCES Claim(Id)
);
