import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../auth/auth.service';
import { ProductService } from '../../products/product.service';
import { PaginatedList } from "../../products/PaginatedList";
import { Product } from "../../products/Product";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {

  clearProducts() {
    this.products = null;
  }

  products: PaginatedList<Product> | null = null;
  loading = false;
  error: string | null = null;

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loading = true;
    this.productService.getProducts().subscribe({
      next: (products) => {
        this.products = products;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching products:', error);
        this.error = 'Failed to load products';
        this.loading = false;
      }
    });
  }
}
