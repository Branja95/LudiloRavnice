export class RentVehicle {
    serviceId: string;
    id: string;
    name: string;
    logoImage: string;
    emailAddress: string;
    description: string;

    constructor(serviceId: string, id: string, name: string, logoImage: string, emailAddress: string, description: string) {
        this.serviceId = serviceId,
        this.name = name;
        this.logoImage = logoImage;
        this.emailAddress = emailAddress;
        this.description = description;
    }
}