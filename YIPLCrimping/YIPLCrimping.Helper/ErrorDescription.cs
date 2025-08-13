namespace YIPLCrimping.Helper
{
    public static class ErrorDescription
    {
        public const string EmployeeIdAlreadyExists_1001 = "Employee ID '{0}' already exists.";
        public const string FailedToAddUser_1002 = "Failed to add user. Please try again later.";
        public const string EmployeeIdIsRequired_1003 = "Employee ID is required.";
        public const string UserNameIsRequired_1004 = "User name is required.";
        public const string UserNameInvalid_1005 = "User name should not contain numbers.";
        public const string RoleCodeIsRequired_1006 = "Role code is required.";
        public const string PlantIsRequired_1007 = "Plant is required.";
        public const string DepartmentIdIsRequired_1008 = "Department ID is required.";
        public const string CreatedByIdIsRequired_1009 = "Created by ID is required.";
        public const string UserNotFound_1010 = "User with employee id '{0}' is not found.";
        public const string FailedToUpdateUser_1011 = "Failed to update user. Please try again later.";
        public const string EmailInvalid_1012 = "Please provide a valid Email address";

        // --- Machine-related descriptions (starting from 1013) ---
        public const string MachineIdIsRequired_1013 = "Machine Id is required.";

        public const string MachineCostInvalid_1014 = "Machine Cost must be greater than zero.";
        public const string MachinePlantIdIsRequired_1015 = "Valid PlantId is required.";
        public const string MachineCreatedByIdRequired_1016 = "CreatedById is required.";
        public const string MachineNameAlreadyExists_1017 = "Machine with name '{0}' already exists.";
        public const string FailedToAddMachine_1018 = "Failed to add machine.";
        public const string FailedToUpdateMachine_1019 = "Failed to update machine. Please try again later.";
        public const string MachineNotFound_1020 = "Machine with id '{0}' is not found.";
        public const string NoMachineRecordsProvided_1020 = "No machine records provided.";
        public const string MachineBulkAddOrUpdateFailed_1021 = "Machine bulk add or update failed.";
        public const string MachineBulkUploadFailed_1024 = "Machine bulk upload failed.";
        public const string MachineNameIsRequired_1025 = "Machine name is required.";

        // --- Plant-related descriptions (starting from 1013) ---
        public const string PlantCodeIsRequired_1026 = "Plant code is required and should be at least 3 characters long";

        public const string PlantNameIsRequired_1027 = "Plant name is required.";
        public const string PlantCreatedByIdRequired_1028 = "Created by ID is required for plant.";
        public const string PlantCodeAlreadyExists_1029 = "Plant code '{0}' already exists.";
        public const string FailedToAddPlant_1030 = "Failed to add plant. Please try again later.";
        public const string FailedToUpdatePlant_1031 = "Failed to update plant. Please try again later.";
        public const string PlantNotFound_1032 = "Plant with code '{0}' not found.";
        public const string NoPlantRecordsProvided_1033 = "No plant records provided for bulk add or update.";
        public const string PlantBulkAddOrUpdateFailed_1034 = "Failed to process bulk add or update for plant records.";
        public const string NoFileUploaded_1035 = "No file uploaded for plant import.";
        public const string InvalidOrEmptyExcelData_1036 = "The uploaded Excel file is empty or invalid.";
        public const string PlantBulkUploadFailed_1037 = "Failed to process bulk plant upload from Excel.";
        public const string NoUserRecordsProvided_1035 = "No user records provided for bulk operation";
        public const string UserBulkAddOrUpdateFailed_1036 = "Failed to bulk add/update users";
        public const string UserValidationFailed_1037 = "User validation failed";
        // --- Template file related (starting from 1038) ---
        public const string FileIsRequired_1038 = "File is required.";

        public const string MasterNameIsInvalid_1039 = "Invalid MasterName. Allowed values are: CrimpingShapes, Customer, Department, Machine, Plant, Supplier, WireSize, WireType.";
        public const string CreatedByIsRequired_1040 = "CreatedBy is required.";
        public const string TemplateAlreadyExists_1041 = "Template file for this master already exists.";
        public const string TemplateNotFound_1042 = "Template was not found.";
        public const string FailedToAddTemplate_1043 = "Failed to save template.";
        public const string FailedToUpdateTemplate_1044 = "Failed to update template.";
        public const string IdIsRequiredForUpdate_1045 = "Id is required for update or delete the template.";
        public const string FailedToDeleteTemplate_1046 = "Failed to delete template.";
        public const string IdIsRequired_1047 = "Please provide Id";
        public const string CityIsRequired_1048 = "Please provide valid city.";

        // --- Department-related descriptions (starting from 1013) ---
        public const string DepartmentNameIsRequired_1201 = "Department name is required.";

        public const string DepartmentAlreadyExists_1202 = "Department '{0}' already exists.";
        public const string FailedToAddDepartment_1203 = "Failed to add department. Please try again later.";
        public const string DepartmentNotFound_1204 = "Department with ID '{0}' is not found.";
        public const string FailedToUpdateDepartment_1205 = "Failed to update department. Please try again later.";
        public const string NoDepartmentRecordsProvided_1206 = "No department records provided for bulk add or update.";
        public const string DepartmentBulkAddOrUpdateFailed_1207 = "Failed to process bulk add or update for department records.";
        public const string DepartmentBulkUploadFailed_1208 = "Failed to process bulk plant upload from Excel.";
        public const string InvalidFileFormat_1209 = "Only Excel files (.xlsx, .xls) are allowed.";
        public const string ModifiedByIdIsRequired_1210 = "Modified by ID is required.";

        // --- Role-related descriptions (starting from 1013) ---
        public const string RoleNameIsRequired_1101 = "Role name is required.";

        public const string RoleNameAlreadyExists_1102 = "Role name '{0}' already exists.";
        public const string FailedToAddRole_1103 = "Failed to add role.";
        public const string RoleIdIsRequired_1104 = "Role ID is required.";
        public const string RoleNotFound_1105 = "Role with ID {0} not found.";
        public const string FailedToUpdateRole_1106 = "Failed to update role.";
        public const string RoleInUseCannotDelete_1107 = "Role is in use by one or more users and cannot be deleted.";
        public const string FailedToDeleteRole_1108 = "Failed to delete role.";

        // --- Shape-related descriptions (starting from 1151) ---
        public const string ShapeNameIsRequired_1151 = "Shape name is required.";

        public const string ShapeNameAlreadyExists_1152 = "Shape name(s) '{0}' already exist.";
        public const string FailedToAddShape_1153 = "Failed to add shape.";
        public const string FailedToUpdateShape_1154 = "Failed to update shape.";
        public const string FailedToDeleteShape_1155 = "Failed to delete shape.";
        public const string ShapeNotFound_1156 = "Shape not found";
        public const string ShapeIdIsRequired_1157 = "Shape ID is required.";
        public const string NoShapeRecordsProvided_1160 = "No shape records were provided for bulk operation.";
        public const string ShapeBulkAddOrUpdateFailed_1161 = "Bulk add/update operation for shapes failed.";
        public const string NoFileUploadedForShape_1162 = "No file was uploaded.";
        public const string InvalidOrEmptyExcelDataForShape_1163 = "Excel file is invalid or contains no valid shape data.";
        public const string ShapeBulkUploadFailed_1164 = "Bulk upload of shapes failed.";

        //Customer related error descriptions
        public const string CustomerNameIsRequired_1050 = "Customer Name Is Required .";

        public const string CustomerCodeIsRequired_1051 = "Customer Code Is Required .";
        public const string CustomerIdIsRequired_1052 = "Customer Id Required.";
        public const string FailedToAddCustomer_1053 = "Failed to add customer. Please try again later.";
        public const string FailedToUpdateCustomer_1054 = "Failed to update customer. Please try again later.";
        public const string CustomerNotFound_1055 = "Customer Not Found.";
        public const string FailedToDeleteCustomer_1056 = "Failed to delete customer. Please try again later.";
        public const string ModifiedByIdIsRequired_1057 = "Modified by ID is required.";
        public const string NoCustomerRecordsProvided_1058 = "No Customer Records Provided ";
        public const string CustomerBulkUploadFailed_1059 = "Customer Bulk Upload Failed ";
        public const string CustomerCodeAlreadyExists_1060 = "Customer code already exists.";

        // WireType specific errors (1401-1499)
        public const string WireTypeIdRequired_1401 = "Wire Type ID is required";

        public const string WireTypeCodeRequired_1402 = "Wire Type Code is required and should be at least 3 characters long";
        public const string WireTypeNameRequired_1403 = "Wire Type Name is required";
        public const string WireTypeNotFound_1404 = "Wire Type not found";
        public const string WireTypeFailedToAdd_1405 = "Failed to add wire type";
        public const string WireTypeFailedToUpdate_1406 = "Failed to update wire type";
        public const string WireTypeFailedToDelete_1407 = "Failed to delete wire type";
        public const string WireTypeModifiedByIdRequired_1408 = "Modified By ID is required for wire type";
        public const string WireTypeAlreadyInactive_1409 = "Wire Type is already inactive";
        public const string NoWireTypeRecordsProvided_1410 = "No wire type records provided";
        public const string WireTypeBulkUploadFailed_1411 = "Wire Type bulk upload failed";
        public const string WireTypeCodeAlreadyExists_1412 = "Wire type code already exists";
        public const string WireTypeCodeDuplicateInBatch_1413 = "Duplicate wire type code found in the same batch";

        public const string WireSizeCodeRequired_1501 = "Wire Size Code is required and should be at least 3 characters long";
        public const string WireSizeInvalid_1502 = "Wire size must be positive";
        public const string WireSizeAddFailed_1503 = "Failed to add wire size";
        public const string DatabaseOperationFailed_1504 = "Database operation failed";
        public const string WireSizeIdRequired_1505 = "Wire size ID is required";
        public const string WireSizeNotFound_1506 = "Wire size not found";
        public const string WireSizeUpdateFailed_1507 = "Failed to update wire size";
        public const string WireSizeAlreadyInactive_1508 = "Wire size is already inactive";
        public const string WireSizeDeleteFailed_1509 = "Failed to delete wire size";
        public const string NoWireSizesProvided_1510 = "No wire sizes provided";
        public const string InvalidExcelFormat_1511 = "Invalid Excel file format";
        public const string NoValidWireSizes_1512 = "No valid wire sizes found";
        public const string WireSizeUploadFailed_1513 = "Wire size upload failed";
        public const string WireSizeCodeAlreadyExists_1514 = "Wire size code already exists";
        public const string WireSizeCodeDuplicateInBatch_1516 = "Duplicate wire size code found in the same batch";

        //shivaji 1250
        //supplier master
        public const string SupplierCodeAlreadyExists_1250 = " Supploer code already exist.";

        public const string SupplierNameAllreadyExist_1251 = " Supplier name already exist.";
        public const string SupplierCodeRequired_1252 = "Supplier Code is required.";
        public const string SupplierNameRequired_1253 = "Supplier Name is required.";
        public const string SupplierNotFound_1254 = "Supplier not found.";
        public const string SupplierFailedToAdd_1255 = "Failed to add supplier";
        public const string SupplierFailedToUpdate_1256 = "Failed to update supplier";
        public const string SupplierFailedToGet_1257 = "Failed to get supplier";
        public const string SupplierModifiedByIdRequired_1258 = "Modified by Id Required";
        public const string UpdatedByIdRequired_1259 = "Update by id required";
        public const string SupplierAlreadyExists_1260 = " Supploer already exist.";

        //shubham 1300
        public const string NoSupplierRecordsProvided_13001 = "No supplier records provided.";

        public const string SupplierNameIsRequired_1302 = "Supplier name is required.";
        public const string SupplierCodeIsRequired_1303 = "Supplier code is required.";
        public const string SupplierNotFound_1304 = "Supplier with given ID was not found.";
        public const string SupplierBulkUploadFailed_1304 = "Supplier bulk upload failed.";

        //shubham 1600 

        // Terminal Import Errors (1600-1699)
        public const string CustomerIsRequired_1601 = "Customer is required";
        public const string PlantIsRequired_1602 = "Plant is required";
        public const string RegistrationNoIsRequired_1603 = "Registration No. is required";
        public const string TerminalNoIsRequired_1604 = "Terminal number is required";
        public const string TerminalAlreadyExists_1605 = "Terminal {0} already exists";
        public const string FailedToAddTerminal_1606 = "Failed to add terminal";
        public const string InvalidMeasurementFormat_1607 = "Invalid measurement format";
        public const string WireCombinationRequired_1608 = "At least one wire combination is required";
        public const string TerminalNotFound_1610 = "Terminal {0} not found";
        public const string TerminalIdIsRequired_1611 = "Terminal ID is required";
        public const string FailedToUpdateTerminal_1612 = "Failed To Update Terminal";
        public const string BulkDataIsRequired_1613 = "Bulk data is required for this operation.";
        public const string FailedToProcessTerminal_1614 = "Failed to process terminal record";
    }
}