export class Comment {
    serviceId: string;
    userId: string;
    dateTime: string;
    text: string;

    constructor(serviceId: string, userId: string, dateTime: string, text: string) {
        this.serviceId = serviceId,
        this.userId = userId;
        this.dateTime = dateTime;
        this.text = text;
    }
}