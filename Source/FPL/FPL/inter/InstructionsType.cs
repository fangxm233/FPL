namespace FPL.inter
{
    public enum InstructionsType
    {
        pushvar,
        pushadr,
        pushval,

        poparg,
        popvar,
        popadr,
        pop,

        add,
        addv,
        add1,
        addv1,
        sub,
        subv,
        sub1,
        subv1,
        div,
        divv,
        mul,
        mulv,

        jmp,
        call,
        ret,

        eqt,
        eqf,
        let,
        lef,
        mot,
        mof,

        endP,

        loadi,
        unloadi,

        func
    }
}
