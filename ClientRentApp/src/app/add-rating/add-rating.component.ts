import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { RatingService } from '../services/rating.service';
import { Rating } from '../models/rating.model';
import { Location } from '@angular/common';

@Component({
  selector: 'app-add-rating',
  templateUrl: './add-rating.component.html',
  styleUrls: ['./add-rating.component.css']
})

export class AddRatingComponent implements OnInit {

  serviceId: string = "-1";

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private ratingService: RatingService, private location: Location) { 
    activatedRoute.params.subscribe(params => {this.serviceId = params["ServiceId"]});
  }

  ngOnInit() {
  }

  onSubmit(form: NgForm, rating: Rating) {

    rating.serviceId = this.serviceId;

    this.ratingService.createRating(rating).subscribe(
      res =>{
        console.log(res);
      }, error =>{
        console.log(error);
      });

    this.location.back();
  }

}
