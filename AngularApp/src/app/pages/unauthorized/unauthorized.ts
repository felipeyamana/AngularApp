import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Navbar } from '../../components/navbar/navbar';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [Navbar],
  templateUrl: './unauthorized.html',
  styleUrl: './unauthorized.scss'
})
export class Unauthorized {
  constructor(private router: Router) {}

  goToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }
}
