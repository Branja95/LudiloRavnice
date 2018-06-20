export class BranchOffice {
    id: number;
    address: string;
    latitude: number;
    longitude: number;
    image: string;

    constructor(id: number, address: string, latitude: number, longitude: number, image: string) {
        this.id = id;
        this.address = address;
        this.latitude = latitude;
        this.longitude = longitude;
        this.image = image;
    }
}