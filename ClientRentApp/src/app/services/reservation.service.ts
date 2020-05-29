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

    this.formData.append('VehicleId', vehicleId);
    this.formData.append('ReservationStart', reservation.reservationStartDate + 'T' + reservation.reservationStartTime);
    this.formData.append('ReservationEnd', reservation.reservationEndDate + 'T' + reservation.reservationEndTime);
    this.formData.append('RentBranchOfficeId', reservation.rentBranchOfficeId);
    this.formData.append('ReturnBranchOfficeId', reservation.returnBranchOfficeId)
    
    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.post("https://localhost:44383/api/Reservation/PostReservation", this.formData, { headers: headers });
  }

}
