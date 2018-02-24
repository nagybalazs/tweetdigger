import * as moment from 'moment';

export class User {
    verified: boolean;
    followerCount: number;
    friendsCount: number;
    listedCount: number;
    favouritesCount: number;
    statusesCount: number;
    name: string;
    screenName: string;
    createdAt: moment.Moment;
    profileImageUrl400x400: string;
    get profileUrl(): string {
        return `https://twitter.com/${this.screenName}`;
    }

    static create(user: User): User {
        let created = new User();

        created.name = user.name;
        created.screenName = user.screenName;
        created.profileImageUrl400x400 = user.profileImageUrl400x400;

        created.verified = user.verified;

        created.followerCount = +user.followerCount;
        if (isNaN(created.followerCount)) {
            created.followerCount = 0;
        }

        created.friendsCount = +user.friendsCount;
        if (isNaN(created.friendsCount)) {
            created.friendsCount = 0;
        }

        created.listedCount = +user.listedCount;
        if (isNaN(created.listedCount)) {
            created.listedCount = 0;
        }

        created.statusesCount = +user.statusesCount;
        if (isNaN(created.statusesCount)) {
            created.statusesCount = 0;
        }

        created.createdAt = moment.utc(user.createdAt);

        user = created;
        return user;
    }
}