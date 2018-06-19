export class Vehicle {
    model: string;
    manufactor: string;
    vehicleType: string;
    yearMade: string;
    description: string;
    pricePerHour: number;
    isAvailable: string;

    constructor(model: string, manufactor: string, vehicleType: string, yearMade: string, description: string, pricePerHour: number, isAvailable: string) {
        this.model = model;
        this.manufactor = manufactor;
        this.vehicleType = vehicleType;
        this.yearMade = yearMade;
        this.description = description;
        this.pricePerHour = pricePerHour;
        this.isAvailable = isAvailable;
    }
}