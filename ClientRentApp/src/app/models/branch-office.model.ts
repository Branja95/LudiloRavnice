export class BranchOffice {
    serviceId: string;
    address: string;
    Latitude: number;
    Longitude: number;
    image: string;

    constructor(serviceId: string, address: string, latitude: number, longitude: number, image: string) {
        this.serviceId = serviceId;
        this.address = address;
        this.Latitude = latitude;
        this.Longitude = longitude;
        this.image = image;
    }
}