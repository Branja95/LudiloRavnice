import { Component, OnInit } from '@angular/core';

import { NgForm, FormsModule } from '@angular/forms';
import {
  Router,
  ActivatedRoute
} from '@angular/router';

import { BranchOfficeService } from '../services/branch-office.service';
import { ReservationService } from '../services/reservation.service';

import { BranchOffice } from '../models/branch-office.model';
import { Reservation } from '../models/reservation.model';

@Component({
  selector: 'app-reserve-a-vehicle',
  templateUrl: './reserve-a-vehicle.component.html',
  styleUrls: ['./reserve-a-vehicle.component.css']
})
export class ReserveAVehicleComponent implements OnInit {

  VehicleId: string = "-1";

  branchOffices = Array<BranchOffice>()

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private branchOfficeService: BranchOfficeService, private reservationService: ReservationService) { 
    activatedRoute.params.subscribe(params => {this.VehicleId = params["VehicleId"]});
  }

  ngOnInit() {
    this.branchOfficeService.getVehicleServiceBranchOffices(this.VehicleId).subscribe(
      data => {
        this.branchOffices = data as Array<BranchOffice>;
      },error => {
        alert(error.error.Message);
      });
  }

  onSubmit(form: NgForm, reservation: Reservation) {
    this.reservationService.createReservation(reservation, this.VehicleId)
    .subscribe(
      data => {
        alert(data);
      }, error => {
        console.log(error);
      }
    );

    this.router.navigateByUrl("/Vehicle");
  }

}
