namespace shared.Model;

public class DagligSkæv : Ordination {
    public List<Dosis> doser { get; set; } = new List<Dosis>();

    public DagligSkæv(DateTime startDen, DateTime slutDen, Laegemiddel laegemiddel) : base(laegemiddel, startDen, slutDen) {
	}

    public DagligSkæv(DateTime startDen, DateTime slutDen, Laegemiddel laegemiddel, Dosis[] doser) : base(laegemiddel, startDen, slutDen) {
        this.doser = doser.ToList();
    }    

    public DagligSkæv() : base(null!, new DateTime(), new DateTime()) {
    }

	public void opretDosis(DateTime tid, double antal) {
        doser.Add(new Dosis(tid, antal));
    }

	public override double samletDosis() {
		return base.antalDage() * doegnDosis();
	}

	public override double doegnDosis() {
        // TODO: Implement!
        //Completed
        if (doser.Any(dosis => dosis.antal < 0))
        {
            throw new ArgumentNullException("Antallet af doser kan ikke være negativt.");
        }

            return doser.Sum(dosis => dosis.antal);
	}

	public override String getType() {
		return "DagligSkæv";
	}
}
