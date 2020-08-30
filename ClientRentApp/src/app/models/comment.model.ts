export class Comment {
    serviceId: string;
    userId: string;
    DateTime: string;
    text: string;
    userFirstName: string;
    userLastName: string;

    constructor(serviceId: string, userId: string, dateTime: string, text: string, userFistName: string, userLastName: string) {
        this.serviceId = serviceId,
        this.userId = userId;
        this.DateTime = dateTime;
        this.text = text;
        this.userFirstName = userFistName;
        this.userLastName = userLastName;
    }
}