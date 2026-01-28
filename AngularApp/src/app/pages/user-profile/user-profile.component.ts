import { Component, ChangeDetectorRef, OnInit  } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../components/navbar/navbar';
import { UserService, User } from '../../services/user.service';

@Component({
  standalone: true,
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss'],
  imports: [CommonModule, Navbar, FormsModule],
})
export class UserProfileComponent implements OnInit {
  profileData: Partial<User> = {};  // <-- store user info
  savedProfileData: Partial<User> = {};
  isDataChanged = false;
  activeMenu: string = 'profile';

  constructor(
    private userService: UserService,
    private ref: ChangeDetectorRef
  ) {}

  onDataChanged() {
    this.isDataChanged = true;
  }

  cancel() {
    this.profileData = { ...this.savedProfileData }; // revert to last saved
    this.isDataChanged = false;
  }

  save() {
    this.userService.updateUser({
    id:  this.profileData.id!,
    firstName: this.profileData.firstName!,
    lastName: this.profileData.lastName!,
    email: this.profileData.email!
  }).subscribe({
    next: updated => {
      console.log('User updated:', updated);

      this.profileData = { ...updated };
      this.savedProfileData = { ...updated };
      this.isDataChanged = false;
    },
    error: err => console.error('Update failed:', err)
  });


    this.savedProfileData = { ...this.profileData }; // store snapshot
    this.isDataChanged = false;
  }

  ngOnInit() {
    this.userService.currentUser$.subscribe(user => {
      if (user) {
        this.profileData.id = user.id;
        this.profileData.firstName = user.firstName;
        this.profileData.lastName = user.lastName;
        this.profileData.email = user.email;
        this.ref.detectChanges();
      }
    });
  }
}

