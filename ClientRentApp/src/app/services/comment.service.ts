import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable({
  providedIn: 'root'
})
export class CommentService {

  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }

  getMethodGetComment(commentId : string){
    return this.httpClient.get("https://localhost:5001/api/Comments/GetComment?id=" + commentId);
  }

  getMethodUsername(commentId): Observable<any>{
    return this.httpClient.get("https://localhost:5001/api/Comments/UserName?commentId=" + commentId);
  }

  getMethodGetUserName(commentId : string){
    return this.httpClient.get("https://localhost:5001/api/Comments/GetUserName?commentId=" + commentId);
  }

  putMethodEditComment(comment): Observable<any> {
    this.formData = new FormData();

    this.formData.append('Text', comment.Text)

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.put("https://localhost:5001/api/Comments/PutComment?commentId=" + comment.Id, this.formData, { headers: headers });
  }

  postMethodCreateComment(comment): Observable<any>{
    this.formData = new FormData();

    this.formData.append('serviceId',comment.serviceId);
    this.formData.append('text', comment.text);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.post("https://localhost:5001/api/Comments/PostComment", this.formData, { headers: headers }); 
  }

}
