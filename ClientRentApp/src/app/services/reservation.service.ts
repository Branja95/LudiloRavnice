import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Reservation } from '../models/reservation.model';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {

  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }

  createReservation(reservation: Reservation, vehicleId): Observable<any> {
    this.formData = new FormData();

    this.formData.append('vehicleId', vehicleId);
    this.formData.append('rentBranchOfficeId', reservation.rentBranchOfficeId);
    this.formData.append('returnBranchOfficeId', reservation.returnBranchOfficeId)
    this.formData.append('reservationStart', reservation.reservationStartDate + 'T' + reservation.reservationStartTime);
    this.formData.append('reservationEnd', reservation.reservationEndDate + 'T' + reservation.reservationEndTime);
    
    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.post("https://localhost:44383/api/Reservation/PostReservation", this.formData, { headers: headers });
  }

}
