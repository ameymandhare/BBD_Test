using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YIPLCrimping.DAL.Models
{
    public class CrimpingStandardRequestVM
    {
        public int Id { get; set; } = 0;

        public string Customer { get; set; }
        public string Flag { get; set; }
        public string Plant { get; set; }
        public string RegistrationNo { get; set; }
        public string ManufacturinCrimpNo { get; set; }
        public string TerminalNo { get; set; }
        public string CommonTerminalNo { get; set; }
        public string TerminalName { get; set; }
        public decimal? TerminalThickness { get; set; }
        public int? CreatedById { get; set; }
        public int? ModifiedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public AccessoryDto Accessories { get; set; }
        public ApplicatorDetailDto ApplicatorDetails { get; set; }
        public List<WireCombinationDto> WireCombinations { get; set; } = new List<WireCombinationDto>();
        public CrimpingStandardDetailDto CrimpingStandardDetails { get; set; }
        public StripingDetailDto StripingDetails { get; set; }
        public CrimpingOtherParameterDto CrimpingOtherParameters { get; set; }
        public CrimpingDiesDetailDto CrimpingDiesDetails { get; set; }
        public ApprovalDetailDto ApprovalDetails { get; set; }
        public TerminalSupplierCrimpingSpecDto TerminalSupplierCrimpingSpec { get; set; }
    }

    public class AccessoryDto
    {
        public int Id { get; set; } = 0;

        public string ShieldNo { get; set; }
        public decimal? RubberSealPosition { get; set; }
    }

    public class ApplicatorDetailDto
    {
        public int Id { get; set; } = 0;

        public string Machine { get; set; }
        public string Feed { get; set; }
    }

    public class WireCombinationDto
    {
        public int Id { get; set; } = 0;

        public string WireCode { get; set; }
        public string WireType { get; set; }
        public string WireSizeCode { get; set; }
        public string WireSize { get; set; }
    }

    public class CrimpingStandardDetailDto
    {
        public int Id { get; set; } = 0;

        public string ApplicableWireSize { get; set; }
        public string InsulationCrimpShape { get; set; }
        public string Ccheight { get; set; }
        public string Icheight { get; set; }
        public string Ccwidth { get; set; }
        public string Icwidth { get; set; }
        public decimal? TensileForceKgf { get; set; }
        public decimal? TensileForceN { get; set; }
        public decimal? PillShape { get; set; }
        public decimal? Soldering { get; set; }
    }

    public class StripingDetailDto
    {
        public int Id { get; set; } = 0;

        public decimal? MiddelStriping { get; set; }
        public decimal? MiddelStrippingUpperLimit { get; set; }
        public decimal? MiddelStrippingLowerLimit { get; set; }
        public decimal? EndStriping { get; set; }
        public decimal? EndStripingUpperLimit { get; set; }
        public decimal? EndStripingLowerLimit { get; set; }
    }

    public class CrimpingOtherParameterDto
    {
        public int Id { get; set; } = 0;

        public string FrontCuttingCarry { get; set; }
        public string RearCuttingCarry { get; set; }
        public string BrushLength { get; set; }
        public string FrontBellMouth { get; set; }
        public string RearBellMouth { get; set; }
        public decimal? BendUp { get; set; }
        public string BendUpUnit { get; set; }
        public decimal? BendDown { get; set; }
        public string BendDownUnit { get; set; }
        public decimal? Rolling { get; set; }
        public string RollingUnit { get; set; }
        public decimal? Twist { get; set; }
        public string TwistUnit { get; set; }
    }

    public class CrimpingDiesDetailDto
    {
        public int Id { get; set; } = 0;

        public string CrimpingDieNoAnvilA { get; set; }
        public string CrimpingDieNoWireCrimperW { get; set; }
        public string CrimpingDieNoInsulationCrimperI { get; set; }
        public decimal? CrimpingDieNoStabilizerCrimperQ { get; set; }
        public decimal? DiesCrimpingWidthConductorAnvilA { get; set; }
        public decimal? DiesCrimpingWidthInsulationAnvilA { get; set; }
        public decimal? DiesCrimpingWidthWireCrimperW { get; set; }
        public decimal? DiesCrimpingWidthInsulationCrimperI { get; set; }
        public decimal? ConductorDieThickness { get; set; }
        public decimal? InsulationDieThickness { get; set; }
    }

    public class ApprovalDetailDto
    {
        public int Id { get; set; } = 0;

        public int? RevisionNo { get; set; }
        public DateTime? RevisionDate { get; set; }
        public string RevisionDetails { get; set; }
        public string MadeBy { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }
    }

    public class TerminalSupplierCrimpingSpecDto
    {
        public int Id { get; set; } = 0;

        public string TerminalSupplierName { get; set; }
        public string TerminalSupplierNumber { get; set; }
        public string ApplicableWireSize { get; set; }
        public string InsulationCrimpShape { get; set; }
        public string Ccheight { get; set; }
        public string Icheight { get; set; }
        public string Ccwidth { get; set; }
        public string Icwidth { get; set; }
        public decimal? TensileForceKgf { get; set; }
        public decimal? TensileForceN { get; set; }
        public string StandardAsperSupplier { get; set; }
    }
}

