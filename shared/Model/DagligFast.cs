namespace shared.Model;

using System.Diagnostics.CodeAnalysis;
using static shared.Util;

public class DagligFast : Ordination {
	
	public Dosis MorgenDosis { get; set; } = new Dosis();
    public Dosis MiddagDosis { get; set; } = new Dosis();
    public Dosis AftenDosis { get; set; } = new Dosis();
    public Dosis NatDosis { get; set; } = new Dosis();

	public DagligFast(DateTime startDen, DateTime slutDen, Laegemiddel laegemiddel, double morgenAntal, double middagAntal, double aftenAntal, double natAntal) : base(laegemiddel, startDen, slutDen) {
        MorgenDosis = new Dosis(CreateTimeOnly(6, 0, 0), morgenAntal);
        MiddagDosis = new Dosis(CreateTimeOnly(12, 0, 0), middagAntal);
        AftenDosis = new Dosis(CreateTimeOnly(18, 0, 0), aftenAntal);
        NatDosis = new Dosis(CreateTimeOnly(23, 59, 0), natAntal);
	}

    public DagligFast() : base(null!, new DateTime(), new DateTime()) {
    }

	public override double samletDosis() {
		
		return base.antalDage() * doegnDosis();
	}

	public override double doegnDosis() {
		// TODO: Implement!
		// Completed
		double retur = -1;

		if (MorgenDosis.antal >= 0 || MiddagDosis.antal >= 0 || AftenDosis.antal >= 0 || NatDosis.antal >= 0)
		{
			retur = MorgenDosis.antal + MiddagDosis.antal + AftenDosis.antal + NatDosis.antal;
        }
            return retur;
        
		
		
       
	}
	
	public Dosis[] getDoser() {
		Dosis[] doser = {MorgenDosis, MiddagDosis, AftenDosis, NatDosis};
		return doser;
	}

	public override String getType() {
		return "DagligFast";
	}
}
