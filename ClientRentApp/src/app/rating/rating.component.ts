import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { RentVehicleService } from '../services/rent-vehicle.service';
import { Rating} from '../models/rating.model';

@Component({
  selector: 'app-rating',
  templateUrl: './rating.component.html',
  styleUrls: ['./rating.component.css']
})

export class RatingComponent implements OnInit {

  serviceId: string = "-1";
  ratings: Array<Rating>;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private rentVehicleService: RentVehicleService) { 
    activatedRoute.params.subscribe(params => {this.serviceId = params["ServiceId"]});
  }

  ngOnInit() {
    /* this.rentVehicleService.getRatings(this.serviceId).subscribe(
      res => {
        this.ratings = res as Array<Rating>;
      }, 
      error =>{
        console.log(error);
      }); */
  }

}
