export class BranchOffice {
    address: string;
    latitude: number;
    longitude: number;

    constructor(address: string, latitude: number, longitude: number) {
        this.address = address;
        this.latitude = latitude;
        this.longitude = longitude;
    }
}