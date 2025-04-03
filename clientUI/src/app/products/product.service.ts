import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../environment";
import { Product } from "./Product";
import { PaginatedList } from "./PaginatedList";

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  constructor(private http: HttpClient) { }

  getProducts(): Observable<PaginatedList<Product>> {
    return this.http.get<PaginatedList<Product>>(`${environment.apiUrl}/products`);
  }

  getProductById(id: number): Observable<Product> {
    return this.http.get<Product>(`${environment.apiUrl}/products/${id}`);
  }

  createProduct(product: Product): Observable<Product> {
    return this.http.post<Product>(`${environment.apiUrl}/products`, product);
  }

  updateProduct(id: number, product: Product): Observable<Product> {
    return this.http.put<Product>(`${environment.apiUrl}/products/${id}`, product);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/products/${id}`);
  }
}

