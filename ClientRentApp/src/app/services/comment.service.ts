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
    return this.httpClient.get("https://localhost:44383/api/Comment/GetComment?id=" + commentId);
  }

  getMethodUsername(commentId): Observable<any>{
    return this.httpClient.get("https://localhost:44383/api/Comment/UserName?commentId=" + commentId);
  }

  putMethodEditComment(comment): Observable<any> {
    this.formData = new FormData();

    this.formData.append('id', comment.Id);
    this.formData.append('text', comment.Text);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.put("https://localhost:44383/api/Comment/PutComment", this.formData, { headers: headers });
  }

  postMethodCreateComment(comment): Observable<any>{
    this.formData = new FormData();

    this.formData.append('serviceId', comment.serviceId);
    this.formData.append('text', comment.text);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    return this.httpClient.post("https://localhost:44383/api/Comment/PostComment", this.formData, { headers: headers }); 
  }

}
