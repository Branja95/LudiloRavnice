export class BranchOffice {
    serviceId: string;
    address: string;
    latitude: number;
    longitude: number;
    image: string;

    constructor(serviceId: string, address: string, latitude: number, longitude: number, image: string) {
        this.serviceId = serviceId;
        this.address = address;
        this.latitude = latitude;
        this.longitude = longitude;
        this.image = image;
    }
}