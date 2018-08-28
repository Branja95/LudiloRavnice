export class Reservation {
    reservationStartDate: string;
    reservationStartTime: string;
    reservationEndDate: string;
    reservationEndTime: string;
    rentBranchOfficeId: string;
    returnBranchOfficeId: string;

    constructor(reservationStartDate: string, reservationStartTime: string, reservationEndDate: string, reservationEndTime: string, rentBranchOfficeId: string, returnBranchOfficeId: string) {
        this.reservationStartDate = reservationStartDate;
        this.reservationStartTime = reservationStartTime;
        this.reservationEndDate = reservationEndDate;
        this.reservationEndTime = reservationEndTime;
        this.rentBranchOfficeId = rentBranchOfficeId;
        this.returnBranchOfficeId = returnBranchOfficeId;
    }
}