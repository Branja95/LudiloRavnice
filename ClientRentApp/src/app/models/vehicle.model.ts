export class Vehicle {
    serviceId: string;
    vehicleTypeId: number;
    model: string;
    manufactor: string;
    vehicleType: string;
    YearMade: string;
    description: string;
    pricePerHour: number;
    isAvailable: string;
    images: string;

    constructor(serviceId: string, vehicleTypeId: number, model: string, manufactor: string, vehicleType: string, yearMade: string, description: string, pricePerHour: number, isAvailable: string, images: string) {
        this.serviceId = serviceId;
        this.vehicleTypeId = vehicleTypeId;
        this.model = model;
        this.manufactor = manufactor;
        this.vehicleType = vehicleType;
        this.YearMade = yearMade;
        this.description = description;
        this.pricePerHour = pricePerHour;
        this.isAvailable = isAvailable;
        this.images = images;
    }
}