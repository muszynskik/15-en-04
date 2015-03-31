#include <cstdio>
#include <cstdlib>
#include <unistd.h>
#include <errno.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <netdb.h>

public class StringCalculatorTest {

/**
* @param args
*/
@Test
    public void emptyStringReturnsZero()
    {
        StringCalculator calc = new StringCalculator();
        assertEquals(0, calc.run(""));
        
    }
@Test
public void SingleNumberReturnsTheValue()
{
StringCalculator calc = new StringCalculator();
        assertEquals(100,calc.run("100"));
}
@Test
public void SumOfTwoNumbers()
{
StringCalculator calc = new StringCalculator();
assertEquals(25,calc.run("20,5"));
}
@Test
public void SumOfTwoNumbersSeparatedByNewLine()
{
StringCalculator calc = new StringCalculator();
assertEquals(25,calc.run("20\n5"));
}
@Test
public void ThreeNumbersSum()
{
StringCalculator calc = new StringCalculator();
assertEquals(27,calc.run("20,5,2"));
}
@Rule
public ExpectedException expEx = ExpectedException.none();
@Test
public void NegativeNumbersThrowExpection()
{
expEx.expect(RuntimeException.class);
expEx.expectMessage("Negative");
StringCalculator calc = new StringCalculator();
calc.run("-1");
}
@Test
public void SingleParameter()
{
StringCalculator calc = new StringCalculator();
assertEquals(11,calc.run("//i\n5i6"));
}
}
