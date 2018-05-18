using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pop {

    public enum Sex
    {
        female,
        male
    }

    public Tile workplace;
    public Sex gender;
    public int age;

    public Pop(Sex genderV, int ageV)
    {
        gender = genderV;
        age = ageV;
    }

    public void LeaveJob()
    {
        if(workplace == null)
        {
            return;
        }
        Improvement job = workplace.improvement;
        job.RemoveWorker(this);
        workplace = null;
    }

    public static Sex DetermineSex(int rng)
    {
        if(rng < 5)
        {
            return Sex.female;
        }
        else
        {
            return Sex.male;
        }
    }
}
