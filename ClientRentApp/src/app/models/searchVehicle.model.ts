export class SearchVehicle {
    VehicleTypeId: string;
    PriceFrom: string;
    PriceTo: string;
    Manufactor: string;
    Model: string;

    constructor(vehicleTypeId: string, priceFrom: string, priceTo: string, manufactor: string, model: string){
        this.VehicleTypeId = vehicleTypeId;
        this.PriceFrom = priceFrom;
        this.PriceTo = priceTo;
        this.Manufactor = manufactor;
        this.Model = model;
    }
}