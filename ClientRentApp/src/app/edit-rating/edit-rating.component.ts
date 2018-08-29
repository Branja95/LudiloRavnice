import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute} from '@angular/router';
import { RatingService} from '../services/rating.service';
import { Rating } from '../models/rating.model';

@Component({
  selector: 'app-edit-rating',
  templateUrl: './edit-rating.component.html',
  styleUrls: ['./edit-rating.component.css']
})
export class EditRatingComponent implements OnInit {

  ratingId: string = "-1";
  rating: Rating;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private ratingService: RatingService) {
    activatedRoute.params.subscribe(
      params => { this.ratingId = params["ratingId"];
    });
  }

  ngOnInit() {
    this.ratingService.getMethodGetRating(this.ratingId).subscribe(
      res => {
        this.rating = res as Rating;
      },error => {
        console.log(error);
      });
  }

  onSubmit(form: NgForm) {
    this.ratingService.putMethodEditRating(this.rating).subscribe(
      res => {
        console.log(res);
      }, error => {
        console.log(error);
      });
    
    form.reset();
    this.router.navigate(['/RentVehicle/']);
  }

}
