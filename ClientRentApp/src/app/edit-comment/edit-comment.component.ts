import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute} from '@angular/router';
import { CommentService} from '../services/comment.service';

@Component({
  selector: 'app-edit-comment',
  templateUrl: './edit-comment.component.html',
  styleUrls: ['./edit-comment.component.css']
})
export class EditCommentComponent implements OnInit {

  commentId: string = "-1";
  comment: Comment;
  
  constructor(private router: Router, private activatedRoute: ActivatedRoute, private commentService: CommentService) {
    activatedRoute.params.subscribe(
      params => { this.commentId = params["commentId"];
    });
  }

  ngOnInit() {
    this.commentService.getMethodGetComment(this.commentId).subscribe(
      res => {
        this.comment = res as Comment;
      },error => {
        console.log(error);
      });
  }

  onSubmit(form: NgForm) {
    this.commentService.putMethodEditComment(this.comment)
    .subscribe(
      res => {
        console.log(res);
      }, error => {
        console.log(error);
      });;
    
    form.reset();
    this.router.navigate(['/RentVehicle/']);
  }

}
