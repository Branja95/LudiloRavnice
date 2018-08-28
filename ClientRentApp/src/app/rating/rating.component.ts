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
  
  userRated: boolean = true;
  ratings: Array<Rating>;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private rentVehicleService: RentVehicleService) { 
    activatedRoute.params.subscribe(params => {this.serviceId = params["ServiceId"]});
  }

  ngOnInit() {
    this.rentVehicleService.getMethodRatings(this.serviceId).subscribe(
      res => {
        this.ratings = res as Array<Rating>;
      }, 
      error =>{
        console.log(error);
      });

    this.rentVehicleService.getMethodHasUserRated(this.serviceId).subscribe(
      res => {
        this.userRated = res as boolean;
      }, error =>{
        console.log(error);
      }); 
  }

  hasUserRated(): boolean{
    if(this.userRated)
    {
      return true;
    }
    else 
    {
      return false;
    }
  }

  canUserEditRating(userId) : boolean {
    if(localStorage.getItem("username") == userId)
    {
      return true;
    }
    else
    {
      return false;
    }
  }

}
