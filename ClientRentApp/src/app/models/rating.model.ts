export class Rating {
    serviceId: string;
    userId: string;
    value: string;

    constructor(serviceId: string, userId: string, value: string) {
        this.serviceId = serviceId,
        this.userId = userId;
        this.value = value;
    }
}