using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pop {

    public enum Sex
    {
        female,
        male
    }

    Improvement workplace;
    public Sex gender;
    public int age;

    public Pop(Sex genderV, int ageV)
    {
        gender = genderV;
        age = ageV;
    }

}
