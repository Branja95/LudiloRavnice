export class Vehicle {
    serviceId: string;
    vehicleTypeId: string;
    model: string;
    manufactor: string;
    vehicleType: string;
    yearMade: string;
    description: string;
    pricePerHour: number;
    isAvailable: string;
    images: string;

    constructor(serviceId: string, vehicleTypeId: string, model: string, manufactor: string, vehicleType: string, yearMade: string, description: string, pricePerHour: number, isAvailable: string, images: string) {
        this.serviceId = serviceId;
        this.vehicleTypeId = vehicleTypeId;
        this.model = model;
        this.manufactor = manufactor;
        this.vehicleType = vehicleType;
        this.yearMade = yearMade;
        this.description = description;
        this.pricePerHour = pricePerHour;
        this.isAvailable = isAvailable;
        this.images = images;
    }
}