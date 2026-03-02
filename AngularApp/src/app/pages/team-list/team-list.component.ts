import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../components/navbar/navbar';
import { FormsModule } from '@angular/forms';
import { OnInit } from '@angular/core';
import { UserService, User } from '../../services/user.service';
import { NgbDropdownModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';

type EmploymentStatus = 'Full Time' | 'Part Time';

export interface TeamMember {
  id: string;
  name: string;
  position: string;
  department: string;
  email: string;
  phone: string;
  status: EmploymentStatus;

  officeLocation?: string;
  teamMates?: string[];
  birthday?: string;
  hrYears?: number;
  address?: string;
}

@Component({
  selector: 'app-team-list',
  templateUrl: './team-list.component.html',
  styleUrls: ['./team-list.component.scss'],
  imports: [CommonModule, Navbar, FormsModule, NgbTooltipModule],
})
export class TeamListComponent implements OnInit {

  query = '';
  page = 1;
  pageSize = 8;

  expandedId: string | null = null;
  selected = new Set<string>();

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    console.log('init');
    this.loadUsers();
    }

  users: TeamMember[] = [];

  get filteredUsers() {
    const q = this.query.toLowerCase().trim();
    const filtered = !q
      ? this.users
      : this.users.filter(u =>
          Object.values(u).some(v =>
            (v ?? '').toString().toLowerCase().includes(q)
          )
        );

    const start = (this.page - 1) * this.pageSize;
    return filtered.slice(start, start + this.pageSize);
  }

  loadUsers(): void {
    this.userService.getUsers(this.page).subscribe(apiUsers => {

      console.log('API users:', apiUsers);

      this.users = apiUsers.map(u => ({
        id: u.id,
        name: `${u.firstName} ${u.lastName}`,
        position: u.roles?.length ? u.roles.join(', ') : 'User',
        department: '-',
        email: u.email,
        phone: '-',
        status: 'Full Time' as const
      }));
    });
  }

  toggleExpanded(id: string) {
    this.expandedId = this.expandedId === id ? null : id;
  }

  toggleSelected(id: string, checked: boolean) {
    checked ? this.selected.add(id) : this.selected.delete(id);
  }

  isSelected(id: string) {
    return this.selected.has(id);
  }

  get selectedCount() {
    return this.selected.size;
  }

  clearSelection() {
    this.selected.clear();
  }

  initials(name: string) {
    return name.split(' ')
      .map(x => x[0])
      .join('')
      .toUpperCase();
  }
}