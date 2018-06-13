export class BranchOffice {
    image: string;
    address: string;
    latitude: number;
    longitude: number;

    constructor(image: string, address: string, latitude: number, longitude: number) {
        this.image = image;
        this.address = address;
        this.latitude = latitude;
        this.longitude = longitude;
    }
}