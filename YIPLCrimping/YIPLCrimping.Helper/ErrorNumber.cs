namespace YIPLCrimping.Helper
{
    public class ErrorNumber
    {
        public const int EmployeeIdAlreadyExists = 1001;
        public const int FailedToAddUser = 1002;
        public const int EmployeeIdIsRequired = 1003;
        public const int UserNameIsRequired = 1004;
        public const int UserNameInvalid = 1005;
        public const int RoleCodeIsRequired = 1006;
        public const int PlantIsRequired = 1007;
        public const int DepartmentIdIsRequired = 1008;
        public const int CreatedByIdIsRequired = 1009;
        public const int UserNotFound = 1010;
        public const int FailedToUpdateUser = 1011;
        public const int EmailInvalid = 1012;

        // --- Machine-related errors (starting from 1013) ---
        public const int MachineIdIsRequired = 1013;

        public const int MachineCostInvalid = 1014;
        public const int MachinePlantIdIsRequired = 1015;
        public const int MachineCreatedByIdRequired = 1016;
        public const int MachineNameAlreadyExists = 1017;
        public const int FailedToAddMachine = 1018;
        public const int FailedToUpdateMachine = 1019;
        public const int MachineNotFound = 1020;
        public const int NoMachineRecordsProvided = 1020;
        public const int MachineBulkAddOrUpdateFailed = 1021;
        public const int MachineBulkUploadFailed = 1024;
        public const int MachineNameIsRequired = 1025;

        // --- Plant-related errors (starting from 1013) ---
        public const int PlantCodeIsRequired = 1026;

        public const int PlantNameIsRequired = 1027;
        public const int PlantCreatedByIdRequired = 1028;
        public const int PlantCodeAlreadyExists = 1029;
        public const int FailedToAddPlant = 1030;
        public const int FailedToUpdatePlant = 1031;
        public const int PlantNotFound = 1032;
        public const int NoPlantRecordsProvided = 1033;
        public const int PlantBulkAddOrUpdateFailed = 1034;
        public const int NoFileUploaded = 1035;
        public const int InvalidOrEmptyExcelData = 1036;
        public const int PlantBulkUploadFailed = 1037;
        public const int NoUserRecordsProvided = 1035;
        public const int UserBulkAddOrUpdateFailed = 1036;
        public const int UserValidationFailed = 1037;

        // --- Template file related (starting from 1038) ---
        public const int FileIsRequired = 1038;

        public const int MasterNameIsInvalid = 1039;
        public const int CreatedByIsRequired = 1040;
        public const int TemplateAlreadyExists = 1041;
        public const int TemplateNotFound = 1042;
        public const int FailedToAddTemplate = 1043;
        public const int FailedToUpdateTemplate = 1044;
        public const int IdIsRequiredForUpdate = 1045;
        public const int FailedToDeleteTemplate = 1046;
        public const int IdIsRequired = 1047;
        public const int CityIsRequired = 1048;

        // --- Department-related errors (starting from 1201) ---
        public const int DepartmentNameIsRequired = 1201;

        public const int DepartmentAlreadyExists = 1202;
        public const int FailedToAddDepartment = 1203;
        public const int DepartmentNotFound = 1204;
        public const int FailedToUpdateDepartment = 1205;
        public const int NoDepartmentRecordsProvided = 1206;
        public const int DepartmentBulkAddOrUpdateFailed = 1207;
        public const int DepartmentBulkUploadFailed = 1208;
        public const int InvalidFileFormat = 1209;

        // --- Shape-related errors (starting from 1151) ---
        public const int ShapeNameIsRequired = 1151;

        public const int ShapeNameAlreadyExists = 1152;
        public const int FailedToAddShape = 1153;
        public const int FailedToUpdateShape = 1154;
        public const int FailedToDeleteShape = 1155;
        public const int ShapeNotFound = 1156;
        public const int ShapeIdIsRequired = 1157;
        public const int NoShapeRecordsProvided = 1160;
        public const int ShapeBulkAddOrUpdateFailed = 1161;
        public const int NoFileUploadedForShape = 1162;
        public const int InvalidOrEmptyExcelDataForShape = 1163;
        public const int ShapeBulkUploadFailed = 1164;

        // --- Role-related errors (starting from 1101) ---
        public const int RoleNameIsRequired = 1101;

        public const int RoleNameAlreadyExists = 1102;
        public const int FailedToAddRole = 1103;
        public const int RoleIdIsRequired = 1104;
        public const int RoleNotFound = 1105;
        public const int FailedToUpdateRole = 1106;
        public const int RoleInUseCannotDelete = 1107;
        public const int FailedToDeleteRole = 1108;

        // Customer related error numbers
        public const int CustomerNameIsRequired = 1050;

        public const int CustomerCodeIsRequired = 1051;
        public const int CustomerIdIsRequired = 1052;
        public const int FailedToAddCustomer = 1053;
        public const int FailedToUpdateCustomer = 1054;
        public const int CustomerNotFound = 1055;
        public const int FailedToDeleteCustomer = 1056;
        public const int ModifiedByIdIsRequired = 1057;
        public const int NoCustomerRecordsProvided = 1058;
        public const int CustomerBulkUploadFailed = 1059;
        public const int CustomerCodeAlreadyExists = 1060;

        // WireType specific errors (1401-1499)
        public const int WireTypeIdRequired = 1401;

        public const int WireTypeCodeRequired = 1402;
        public const int WireTypeNameRequired = 1403;
        public const int WireTypeNotFound = 1404;
        public const int WireTypeFailedToAdd = 1405;
        public const int WireTypeFailedToUpdate = 1406;
        public const int WireTypeFailedToDelete = 1407;
        public const int WireTypeModifiedByIdRequired = 1408;
        public const int WireTypeAlreadyInactive = 1409;
        public const int NoWireTypeRecordsProvided = 1410;
        public const int WireTypeBulkUploadFailed = 1411;
        public const int WireTypeCodeAlreadyExists = 1412;
        public const int WireTypeCodeDuplicateInBatch = 1413;
        public const int NoValidWireTypesToImport = 1415;

        public const int WireSizeCodeRequired = 1501;
        public const int WireSizeInvalid = 1502;
        public const int WireSizeAddFailed = 1503;
        public const int DatabaseOperationFailed = 1504;
        public const int WireSizeIdRequired = 1505;
        public const int WireSizeNotFound = 1506;
        public const int WireSizeUpdateFailed = 1507;
        public const int WireSizeAlreadyInactive = 1508;
        public const int WireSizeDeleteFailed = 1509;
        public const int NoWireSizesProvided = 1510;
        public const int InvalidExcelFormat = 1511;
        public const int NoValidWireSizes = 1512;
        public const int WireSizeUploadFailed = 1513;
        public const int WireSizeCodeAlreadyExists = 1514;
        public const int WireSizeCodeDuplicateInBatch = 1516;
        public const int NoValidWireSizesToImport = 1519;

        //shivaji 1250
        //supplier master
        public const int SupplierCodeAlreadyExists = 1250;

        public const int SupplierNameAllreadyExist = 1251;
        public const int SupplierCodeRequired = 1252;
        public const int SupplierNameRequired = 1253;
        public const int SupplierNotFound = 1254;
        public const int SupplierFailedToAdd = 1255;
        public const int SupplierFailedToUpdate = 1256;
        public const int SupplierFailedToGet = 1257;
        public const int SupplierModifiedByID = 1258;
        public const int SupplierAlreadyExists = 1259;

        //shubham 1300
        public const int NoSupplierRecordsProvided = 1301;

        public const int SupplierNameIsRequired = 1302;
        public const int SupplierCodeIsRequired = 1303;
        public const int SupplierBulkUploadFailed = 1304;

        //shubham 1600
        // Terminal Import Errors (1600-1699)
        public const int CustomerIsRequired = 1601;
        public const int PlantIdIsRequired = 1602;
        public const int RegistrationNoIsRequired = 1603;
        public const int TerminalNoIsRequired = 1604;
        public const int TerminalAlreadyExists = 1605;
        public const int FailedToAddTerminal = 1606;
        public const int InvalidMeasurementFormat = 1607;
        public const int WireCombinationRequired = 1608;
        public const int TerminalNotFound = 1610;
        public const int TerminalIdIsRequired = 1611;
        public const int FailedToUpdateTerminal = 1612;
        public const int BulkDataIsRequired = 1613;
        public const int FailedToProcessTerminal = 1614;
    }
}