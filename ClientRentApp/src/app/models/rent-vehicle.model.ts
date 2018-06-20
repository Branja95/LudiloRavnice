export class RentVehicle {
    name: string;
    logoImage: string;
    emailAddress: string;
    description: string;

    constructor(name: string, logoImage: string, emailAddress: string, description: string) {
        this.name = name;
        this.logoImage = logoImage;
        this.emailAddress = emailAddress;
        this.description = description;
    }
}